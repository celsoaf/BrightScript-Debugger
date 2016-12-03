using System;
using System.Collections.Generic;
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
            _rokuController.OnOutput += _rokuController_OnOutput;
            CommandFactory = new CommandFactory(_rokuController);

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
            throw new NotImplementedException();
        }

        public async Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            Close();
        }

        public void Close()
        {
            _rokuController.Close();
        }

        private void _rokuController_OnOutput(string obj)
        {
            WorkerThread.PostOperation(async () => AD7OutputDebugStringEvent.Send(Engine, obj));
        }
    }
}