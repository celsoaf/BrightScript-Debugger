namespace BrightScript.Debugger
{
    internal enum CommandKind
    {
        Breakpoint,
        Step,
        Continue,
        Detach,
        DebuggerEnvironmentReady
    }

    internal class Command
    {
        public CommandKind Kind { get; private set; }

        public Command(CommandKind command)
        {
            this.Kind = command;
        }
    }

    internal class BreakpointCommand : Command
    {
        public string File { get; private set; }
        public int LineNumber { get; private set; }

        public BreakpointCommand(string file, int lineNumber) : base(CommandKind.Breakpoint)
        {
            this.File = file;
            this.LineNumber = lineNumber;
        }
    }
}