using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Interfaces
{
    internal interface ICommandFactory
    {
        uint Radix { get; }
        bool SupportsFrameFormatting { get; }

        Task<bool> SetRadix(uint radix);
        Task<bool> SetJustMyCode(bool enabled);
        Task<string> DataEvaluateExpression(string expr, int threadId, uint frame);

        Task ExecStepInto(int threadId);
        Task ExecStepOver(int threadId);
        Task ExecStepOut(int threadId);
        Task ExecContinue(int threadId);

        Task<List<ThreadContext>> GetStackTrace();
        Task<List<VariableInformation>> GetVariables();

        bool CanDetach();
    }
}