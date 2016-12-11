using System.Collections.ObjectModel;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IEngineCallback
    {
        void OnErrorImmediate(string message);
        void OnProcessExit(uint exitCode);

        void Send(IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program, IDebugThread2 thread);
        void Send(IDebugEvent2 eventObject, string iidEvent, IDebugThread2 thread);

        void OnExpressionEvaluationComplete(IVariableInformation var, IDebugProperty2 prop = null);

        void OnThreadStart(DebuggedThread thread);
        void OnThreadExit(DebuggedThread thread, uint exitCode);
        void OnEntryPoint(DebuggedThread thread);
        void OnBreakpoint(DebuggedThread thread, ReadOnlyCollection<object> clients);

        void Close();
    }
}