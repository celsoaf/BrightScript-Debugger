﻿using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;

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

        public bool CanDetach()
        {
            return true;
        }
    }
}