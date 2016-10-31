using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;

namespace BrightScript.Debugger.Core.CommandFactories
{
    public class RokuMICommandFactory : MICommandFactory
    {
        public override string Name { get; }

        protected override async Task<Results> ThreadFrameCmdAsync(string command, ResultClass expectedResultClass, int threadId, uint frameLevel)
        {
            return await _debugger.CmdAsync(command, expectedResultClass);
        }

        protected override Task<Results> ThreadCmdAsync(string command, ResultClass expectedResultClass, int threadId)
        {
            return _debugger.CmdAsync(command, expectedResultClass);
        }

        public override bool SupportsStopOnDynamicLibLoad()
        {
            return false;
        }

        public override bool AllowCommandsWhileRunning()
        {
            return false;
        }

        public override async Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            return null;
        }

        public override async Task EnableTargetAsyncOption()
        {
            
        }
    }
}