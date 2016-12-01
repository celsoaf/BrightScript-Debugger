using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IDebuggedProcess : ITransportCallback
    {
        AD_PROCESS_ID Id { get; }
        ICommandFactory CommandFactory { get; }
        IWorkerThread WorkerThread { get; }
        IThreadCache ThreadCache { get; }

        Task Initialize(EventWaitHandle waitLoop, CancellationToken token);
        Task ResumeFromLaunch();
        Task CmdDetach();
        Task<List<ulong>> StartAddressesForLine(string file, uint line);
        Task Step(int threadId, enum_STEPKIND kind, enum_STEPUNIT unit);
        Task Execute(DebuggedThread thread);

        Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx);
        Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx);

        void OnPostedOperationError(object sender, Exception e);
        void Terminate();
        void Close();
    }
}