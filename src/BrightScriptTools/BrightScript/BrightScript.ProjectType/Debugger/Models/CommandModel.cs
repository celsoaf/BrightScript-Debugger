using System;
using System.Threading;
using BrightScript.Debugger.Enums;

namespace BrightScript.Debugger.Models
{
    public class CommandModel
    {
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        public CommandModel(CommandType commandType, DebuggerCommandEnum cmd, string arg=null)
        {
            ResultType = commandType;
            Cmd = cmd;
            Arg = arg;
        }

        public DebuggerCommandEnum Cmd { get; }
        public string Arg { get; }

        public CommandType ResultType { get; }

        public Object Result { get; set; }

        public void Wait()
        {
            _waitHandle.WaitOne();
        }

        public void Set()
        {
            _waitHandle.Set();
        }
    }
}