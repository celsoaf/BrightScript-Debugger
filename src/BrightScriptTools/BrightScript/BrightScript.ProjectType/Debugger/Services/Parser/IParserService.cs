using System;
using System.Collections.Generic;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Services.Parser
{
    public interface IParserService
    {
        void Start(int port);
        void Stop();

        void ProcessLog(LogModel log);

        int Port { get; }

        event Action<List<string>> CurrentFunctionProcessed;
        event Action<List<BacktraceModel>> BacktraceProcessed;
        event Action<List<VariableModel>> VariablesProcessed;
        event Action DebugPorcessed;
        event Action AppCloseProcessed;
        event Action AppOpenProcessed;
        event Action<string> ErrorProcessed;
    }
}