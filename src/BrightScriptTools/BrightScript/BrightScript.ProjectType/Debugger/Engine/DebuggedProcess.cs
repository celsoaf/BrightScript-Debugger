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
        public DebuggedProcess(string ip, int port, IEngineCallback engineCallback, IWorkerThread workerThread, AD7Engine engine)
        {
            
        }

        public AD_PROCESS_ID Id { get; }
        public ICommandFactory CommandFactory { get; }
        public IWorkerThread WorkerThread { get; }
        public IThreadCache ThreadCache { get; }

        public Task Execute(DebuggedThread thread)
        {
            throw new NotImplementedException();
        }

        public Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public void OnPostedOperationError(object sender, Exception e)
        {
            throw new NotImplementedException();
        }

        public Task Initialize(EventWaitHandle waitLoop, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task ResumeFromLaunch()
        {
            throw new NotImplementedException();
        }

        public Task CmdDetach()
        {
            throw new NotImplementedException();
        }

        public Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            throw new NotImplementedException();
        }

        public Task Step(int threadId, enum_STEPKIND kind, enum_STEPUNIT unit)
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