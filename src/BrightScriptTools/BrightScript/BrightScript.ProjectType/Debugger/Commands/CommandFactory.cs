using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;

namespace BrightScript.Debugger.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public uint Radix { get; }
        public bool SupportsFrameFormatting { get; }

        public Task<bool> SetRadix(uint radix)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SetJustMyCode(bool enabled)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> DataEvaluateExpression(string expr, int threadId, uint frame)
        {
            throw new System.NotImplementedException();
        }

        public bool CanDetach()
        {
            throw new System.NotImplementedException();
        }
    }
}