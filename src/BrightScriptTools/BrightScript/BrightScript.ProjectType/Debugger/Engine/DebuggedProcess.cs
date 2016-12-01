using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class DebuggedProcess : IDebuggedProcess
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly IEngineCallback _engineCallback;
        
        public DebuggedProcess(string ip, int port, IEngineCallback engineCallback, IWorkerThread workerThread, AD7Engine engine)
        {
            _ip = ip;
            _port = port;
            _engineCallback = engineCallback;
            WorkerThread = workerThread;
            Engine = engine;

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
            throw new NotImplementedException();
        }

        public async Task CmdDetach()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}