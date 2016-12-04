using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.MIDebugEngine;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class EngineCallback : IEngineCallback
    {
        private readonly IDebugEventCallback2 _eventCallback;
        private readonly AD7Engine _engine;

        private ConcurrentQueue<EventModel> _operations = new ConcurrentQueue<EventModel>();
        private Thread _thread;
        private volatile bool _isClosed;

        public EngineCallback(AD7Engine engine, IDebugEventCallback2 ad7Callback)
        {
            _engine = engine;
            _eventCallback = ad7Callback;

            _thread = new Thread(Run);
            _thread.Name = "MIDebugger.EngineCallback";
            _thread.Start();
        }

        private void Run()
        {
            while (!_isClosed)
            {
                EventModel em;
                if (_operations.TryDequeue(out em))
                {
                    SendInternal(em);
                }
                else
                {
                   Thread.Sleep(100);
                }
            }
        }

        public void OnErrorImmediate(string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnProcessExit(uint exitCode)
        {
            AD7ProgramDestroyEvent eventObject = new AD7ProgramDestroyEvent(exitCode);

            try
            {
                Send(eventObject, AD7ProgramDestroyEvent.IID, null);
            }
            catch (InvalidOperationException)
            {
                // If debugging has already stopped, this can throw
            }
        }

        public void Send(IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program, IDebugThread2 thread)
        {
            var model = new EventModel(eventObject, iidEvent, program, thread);

            _operations.Enqueue(model);

            model.Wait();
        }

        private void SendInternal(EventModel model)
        {
            uint attributes;
            Guid riidEvent = new Guid(model.IidEvent);

            EngineUtils.RequireOk(model.EventObject.GetAttributes(out attributes));
            EngineUtils.RequireOk(_eventCallback.Event(_engine, null, model.Program, model.Thread, model.EventObject, ref riidEvent, attributes));

            model.Set();
        }

        public void Send(IDebugEvent2 eventObject, string iidEvent, IDebugThread2 thread)
        {
            IDebugProgram2 program = _engine;
            if (!_engine.ProgramCreateEventSent)
            {
                // Any events before programe create shouldn't include the program
                program = null;
            }

            Send(eventObject, iidEvent, program, thread);
        }

        public void OnExpressionEvaluationComplete(IVariableInformation var, IDebugProperty2 prop = null)
        {
            throw new System.NotImplementedException();
        }

        public void OnThreadExit(DebuggedThread debuggedThread, uint exitCode)
        {
            Debug.Assert(_engine.DebuggedProcess.WorkerThread.IsPollThread());

            AD7Thread ad7Thread = (AD7Thread)debuggedThread.Client;
            Debug.Assert(ad7Thread != null);

            AD7ThreadDestroyEvent eventObject = new AD7ThreadDestroyEvent(exitCode);

            Send(eventObject, AD7ThreadDestroyEvent.IID, ad7Thread);
        }

        public void OnEntryPoint(DebuggedThread thread)
        {
            AD7EntryPointEvent eventObject = new AD7EntryPointEvent();

            Send(eventObject, AD7EntryPointEvent.IID, (AD7Thread)thread.Client);
        }

        public void OnBreakpoint(DebuggedThread thread, ReadOnlyCollection<object> clients)
        {
            IDebugBoundBreakpoint2[] boundBreakpoints = new IDebugBoundBreakpoint2[clients.Count];

            int i = 0;
            foreach (object objCurrentBreakpoint in clients)
            {
                boundBreakpoints[i] = (IDebugBoundBreakpoint2)objCurrentBreakpoint;
                i++;
            }

            // An engine that supports more advanced breakpoint features such as hit counts, conditions and filters
            // should notify each bound breakpoint that it has been hit and evaluate conditions here.
            // The sample engine does not support these features.

            AD7BoundBreakpointsEnum boundBreakpointsEnum = new AD7BoundBreakpointsEnum(boundBreakpoints);

            AD7BreakpointEvent eventObject = new AD7BreakpointEvent(boundBreakpointsEnum);

            AD7Thread ad7Thread = (AD7Thread)thread.Client;
            Send(eventObject, AD7BreakpointEvent.IID, ad7Thread);
        }

        public void OnThreadStart(DebuggedThread debuggedThread)
        {
            // This will get called when the entrypoint breakpoint is fired because the engine sends a thread start event
            // for the main thread of the application.

            AD7ThreadCreateEvent eventObject = new AD7ThreadCreateEvent();
            Send(eventObject, AD7ThreadCreateEvent.IID, (IDebugThread2)debuggedThread.Client);
        }

        public void Close()
        {
            _isClosed = true;
        }
    }
}