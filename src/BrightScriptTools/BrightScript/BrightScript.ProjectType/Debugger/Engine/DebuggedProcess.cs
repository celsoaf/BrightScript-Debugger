using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Commands;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Transport;
using Microsoft.MIDebugEngine;
using Microsoft.VisualStudio.Debugger.Interop;
#pragma warning disable 1998

namespace BrightScript.Debugger.Engine
{
    internal class DebuggedProcess : IDebuggedProcess
    {
        public ProcessState ProcessState { get; private set; }

        private readonly IEngineCallback _engineCallback;
        private readonly IRokuController _rokuController;
        private bool _connected;

        public DebuggedProcess(IPEndPoint endPoint, IEngineCallback engineCallback, IWorkerThread workerThread, AD7Engine engine)
        {
            _engineCallback = engineCallback;
            WorkerThread = workerThread;
            Engine = engine;

            ProcessState = ProcessState.NotConnected;

            _rokuController = new RokuController(endPoint);
            _rokuController.OnOutput += RokuControllerOnOutput;
            _rokuController.OnBackTrace += RokuControllerOnOnBackTrace;
            _rokuController.RunModeEvent += RokuControllerOnRunModeEvent;
            _rokuController.BreakModeEvent += RokuControllerOnBreakModeEvent;
            _rokuController.ProcessExitEvent += RokuControllerOnProcessExitEvent;

            CommandFactory = new CommandFactory(_rokuController);

            ThreadCache = new ThreadCache(engineCallback, Engine, CommandFactory);

            // we do NOT have real Win32 process IDs, so we use a guid
            AD_PROCESS_ID pid = new AD_PROCESS_ID();
            pid.ProcessIdType = (int)enum_AD_PROCESS_ID.AD_PROCESS_ID_GUID;
            pid.guidProcessId = Guid.NewGuid();
            this.Id = pid;
        }

        public AD_PROCESS_ID Id { get; }
        public AD7Engine Engine { get; }
        public ICommandFactory CommandFactory { get; }
        public IWorkerThread WorkerThread { get; }
        public IThreadCache ThreadCache { get; }

        public async Task Execute(DebuggedThread thread)
        {
            ThreadCache.MarkDirty();

            await CommandFactory.ExecContinue(thread.Id);
            ProcessState = ProcessState.Running;
        }

        public async Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx)
        {
            var vars = await CommandFactory.GetVariables();

            var res = new List<VariableInformation>();
            foreach (var variable in vars)
            {
                res.Add(await variable.CreateMIDebuggerVariable(ctx,Engine, thread));
            }

            return res;
        }

        public void OnPostedOperationError(object sender, Exception e)
        {
            throw new NotImplementedException();
        }

        public async Task Initialize()
        {

        }

        public async Task ResumeFromLaunch()
        {
            _rokuController.Connect();
            _connected = true;
        }

        public async Task CmdDetach()
        {
            Close();
        }

        public async Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            throw new NotImplementedException();
        }

        public async Task Step(int threadId, enum_STEPKIND kind, enum_STEPUNIT unit)
        {
            ThreadCache.MarkDirty();

            if ((unit == enum_STEPUNIT.STEP_LINE) || (unit == enum_STEPUNIT.STEP_STATEMENT))
            {
                switch (kind)
                {
                    case enum_STEPKIND.STEP_INTO:
                        await CommandFactory.ExecStepInto(threadId);
                        break;
                    case enum_STEPKIND.STEP_OVER:
                        await CommandFactory.ExecStepOver(threadId);
                        break;
                    case enum_STEPKIND.STEP_OUT:
                        await CommandFactory.ExecStepOut(threadId);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (unit == enum_STEPUNIT.STEP_INSTRUCTION)
            {
                switch (kind)
                {
                    case enum_STEPKIND.STEP_INTO:
                        await CommandFactory.ExecStepInto(threadId);
                        break;
                    case enum_STEPKIND.STEP_OVER:
                        await CommandFactory.ExecStepOver(threadId);
                        break;
                    case enum_STEPKIND.STEP_OUT:
                        await CommandFactory.ExecStepOut(threadId);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            //RokuControllerOnRunModeEvent();
            ProcessState = ProcessState.Running;
        }

        public void Terminate()
        {
            Close();
        }

        public void Close()
        {
            _rokuController.Close();
            _rokuController.OnOutput -= RokuControllerOnOutput;
            _rokuController.RunModeEvent -= RokuControllerOnRunModeEvent;
            _rokuController.BreakModeEvent -= RokuControllerOnBreakModeEvent;
        }

        private void RokuControllerOnOutput(string obj)
        {
            WorkerThread.PostOperation(async () => AD7OutputDebugStringEvent.Send(Engine, obj));
        }

        private void RokuControllerOnOnBackTrace(List<ThreadContext> threadContexts)
        {
            ThreadCache.SetStackFrames(0, threadContexts);
        }

        private void RokuControllerOnRunModeEvent()
        {
            WorkerThread.PostOperation(async () =>
            {
                if (ProcessState == ProcessState.NotConnected)
                {
                    var thread = ThreadCache.FindThread(0);
                    ThreadCache.SendThreadEvents();

                    _engineCallback.OnEntryPoint(thread);
                }
                else
                    ThreadCache.SendThreadEvents();

                ProcessState = ProcessState.Running;
            });
        }

        private void RokuControllerOnBreakModeEvent(int threadId)
        {
            WorkerThread.PostOperation(async () =>
                {
                    if (ProcessState == ProcessState.Running)
                    {
                        var thread = ThreadCache.FindThread(threadId);
                        ThreadCache.SendThreadEvents();

                        ProcessState = ProcessState.Stopped;

                        _engineCallback.OnBreakpoint(thread, new ReadOnlyCollection<object>(new AD7BoundBreakpoint[] { }));
                    }
                });
        }

        private void RokuControllerOnProcessExitEvent()
        {
            ProcessState = ProcessState.Exited;
        }
    }
}