using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Commands
{
    internal class CommandFactory : ICommandFactory
    {
        private readonly IRokuController _rokuController;

        public CommandFactory(IRokuController rokuController)
        {
            _rokuController = rokuController;
        }

        public uint Radix { get; private set; }
        public bool SupportsFrameFormatting { get; }

        public Task<bool> SetRadix(uint radix)
        {
            Radix = radix;

            return Task.FromResult<bool>(true);
        }

        public Task<bool> SetJustMyCode(bool enabled)
        {
            return Task.FromResult<bool>(true);
        }

        public Task<string> DataEvaluateExpression(string expr, int threadId, uint frame)
        {
            throw new System.NotImplementedException();
        }

        public async Task ExecStepInto(int threadId)
        {
            await _rokuController.CmdAsync<object>(new CommandModel(CommandType.Step, DebuggerCommandEnum.s));
        }

        public async Task ExecStepOver(int threadId)
        {
            await _rokuController.CmdAsync<object>(new CommandModel(CommandType.Step, DebuggerCommandEnum.over));
        }

        public async Task ExecStepOut(int threadId)
        {
            await _rokuController.CmdAsync<object>(new CommandModel(CommandType.Step, DebuggerCommandEnum.@out));
        }

        public async Task ExecContinue(int threadId)
        {
            await _rokuController.CmdAsync<object>(new CommandModel(CommandType.NoResult, DebuggerCommandEnum.c));
        }

        public async Task<List<ThreadContext>> GetStackTrace()
        {
            return await _rokuController.CmdAsync<List<ThreadContext>>(new CommandModel(CommandType.Backtrace, DebuggerCommandEnum.bt));
        }

        public async Task<List<VariableInformation>> GetVariables()
        {
            return await _rokuController.CmdAsync<List<VariableInformation>>(new CommandModel(CommandType.Variables, DebuggerCommandEnum.var));
        }

        public bool CanDetach()
        {
            return true;
        }
    }
}