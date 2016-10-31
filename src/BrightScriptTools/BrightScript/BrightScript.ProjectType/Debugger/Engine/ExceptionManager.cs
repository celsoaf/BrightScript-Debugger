using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Core;
using BrightScript.Debugger.Core.CommandFactories;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class ExceptionManager
    {
        // ***************** DESIGN NOTES *************************
        // At least at the time of the creation (VS 2015 RTM), the exception settings dialog pushes
        // multiple redundant updates to the settings when exception change. So if we directly wire
        // up receiving updates via the AD7 interfaces to providing them to the backend debugger
        // then we would make many more calls that would otherwise be necessary. To avoid this problem
        // we instead queue the update when the AD7 call comes in, and flush the queue on a delay.
        // To avoid the possiblity of leaving break state before the queue is flushed, we call
        // back into the exception manager (EnsureSettingsUpdated) to wait for the settings to be flushed.
        //

        private readonly MICommandFactory _commandFactory;
        private readonly WorkerThread _worker;
        private readonly ISampleEngineCallback _callback;
        private bool _initialSettingssSent;

        private readonly object _updateLock = new object();
        private int? _lastUpdateTime;
        private Task _updateTask;
        private CancellationTokenSource _updateDelayCancelSource;

        private class SettingsUpdates
        {
            // Threading note: these are only modified on the main thread
            public ExceptionBreakpointState? NewCategoryState;
            public readonly Dictionary<string, ExceptionBreakpointState> RulesToAdd;
            public readonly HashSet<string> RulesToRemove = new HashSet<string>();

            public SettingsUpdates(/*OPTIONAL*/ ExceptionBreakpointState? initialNewCategoryState, /*OPTIONAL*/ ReadOnlyDictionary<string, ExceptionBreakpointState> initialRuleChanges)
            {
                this.NewCategoryState = initialNewCategoryState;

                // The dictionary constructor which takes a read only dictionary is unhappy if we pass in null, so switch off which constructor we call
                if (initialRuleChanges != null)
                    this.RulesToAdd = new Dictionary<string, ExceptionBreakpointState>(initialRuleChanges);
                else
                    this.RulesToAdd = new Dictionary<string, ExceptionBreakpointState>();
            }
        }

        /// <summary>
        /// Holder class used to hold a settings update + a lock on it, when
        /// the holder is disposed, we will drop the lock and ensure that we have
        /// queued the processing of updates.
        /// </summary>
        private class SettingsUpdateHolder : IDisposable
        {
            /// <summary>
            /// [Required] The SettingsUpdates being held by the holder
            /// </summary>
            public readonly SettingsUpdates Value;
            private readonly ExceptionManager _parent;
            private readonly object _updateLock;

            public SettingsUpdateHolder(SettingsUpdates value, ExceptionManager parent, object updateLock)
            {
                this.Value = value;
                _parent = parent;
                _updateLock = updateLock;
                Monitor.Enter(_updateLock);
            }

            public void Dispose()
            {
                Monitor.Exit(_updateLock);
                _parent.EnsureUpdateTaskStarted();
            }
        }

        public ExceptionManager(MICommandFactory commandFactory, WorkerThread worker, ISampleEngineCallback callback)
        {
            Debug.Assert(commandFactory != null, "Missing commandFactory");
            Debug.Assert(worker != null, "Missing worker");
            Debug.Assert(callback != null, "Missing callback");

            _commandFactory = commandFactory;
            _worker = worker;
            _callback = callback;
        }

        public void RemoveAllSetExceptions(Guid guidType)
        {
            
        }

        public void RemoveSetException(ref EXCEPTION_INFO exceptionInfo)
        {
            
        }

        public void SetException(ref EXCEPTION_INFO exceptionInfo)
        {
            
        }

        private void EnsureUpdateTaskStarted()
        {
            lock (_updateLock)
            {
                // Do nothing until the first call to EnsureSettingsUpdated
                if (_initialSettingssSent)
                {
                    if (_updateTask == null)
                    {
                        _lastUpdateTime = null;
                        _updateDelayCancelSource = new CancellationTokenSource();
                        _updateTask = Task.Run(FlushSettingsUpdates);
                    }
                    else
                    {
                        _lastUpdateTime = Environment.TickCount;
                    }
                }
            }
        }

        /// <summary>
        /// Called before resuming the target process to make sure that all updates have been sent
        /// </summary>
        /// <returns>Task which is signaled when the operation is complete</returns>
        public Task EnsureSettingsUpdated()
        {
            lock (_updateLock)
            {
                if (_updateTask != null)
                {
                    // If we are still delaying our processing, stop delaying it
                    _updateDelayCancelSource.Cancel();

                    return _updateTask;
                }
                else
                {
                    // No task is running, so just return an already signaled task
                    return Task.FromResult<object>(null);
                }
            }
        }

        private async Task FlushSettingsUpdates()
        {
            while (true)
            {
                // Delay sending updates until it has been ~50 ms since we have seen an update
                try
                {
                    while (!_updateDelayCancelSource.IsCancellationRequested)
                    {
                        await Task.Delay(50, _updateDelayCancelSource.Token);

                        lock (_updateLock)
                        {
                            if (_lastUpdateTime.HasValue)
                            {
                                uint millisecondsSinceLastUpdate = unchecked((uint)(Environment.TickCount - _lastUpdateTime.Value));

                                // Clear this so that we don't think there is an unprocessed update at the end
                                _lastUpdateTime = null;

                                // Pick a number slightly less than the ms that we pass to Task.Delay as the resolution on Environment.TickCount is not great
                                // and anyway we aren't trying to be precise as to how long we wait.
                                if (millisecondsSinceLastUpdate >= 45)
                                    break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // Calls to EnsureSettingsUpdated cancel the delay
                }

                lock (_updateLock)
                {
                    if (_lastUpdateTime == null)
                    {
                        // No more updates have been posted since the start of this iteration of the loop, we are done.
                        _updateTask = null;
                        _updateDelayCancelSource = null;
                        break;
                    }
                    else
                    {
                        // An update may have been posted since our last trip arround the category loop, go again
                        _lastUpdateTime = null;
                        continue;
                    }
                }
            }
        }
    }
}