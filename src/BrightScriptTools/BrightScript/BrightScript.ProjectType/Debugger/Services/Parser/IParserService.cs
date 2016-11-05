using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Services.Parser
{
    public interface IParserService
    {
        Task Start(int port);
        void Stop();

        void ProcessLog(LogModel log);

        int Port { get; }

        event Action<int, List<string>> CurrentFunctionProcessed;
        event Action<int, List<BacktraceModel>> BacktraceProcessed;
        event Action<int, List<VariableModel>> VariablesProcessed;
        event Action<int> DebugPorcessed;
        event Action<int> AppCloseProcessed;
        event Action<int> AppOpenProcessed;
        event Action<int, string> ErrorProcessed;
    }
}