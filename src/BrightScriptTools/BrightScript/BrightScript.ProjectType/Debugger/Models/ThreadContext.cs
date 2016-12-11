namespace BrightScript.Debugger.Models
{
    public class ThreadContext
    {
        public ThreadContext(ulong? addr, MITextPosition textPosition, string function, uint level, string from)
        {
            pc = addr;
            sp = 0;
            TextPosition = textPosition;
            Function = function;
            Level = level;
            From = from;
        }

        /// <summary>
        /// [Optional] Program counter. This will be null for an annotated frame.
        /// </summary>
        public ulong? pc { get; private set; }
        public ulong sp { get; private set; }
        public string Function { get; private set; }
        public MITextPosition TextPosition { get; private set; }

        public string From { get; private set; }

        public uint Level { get; private set; }
    }
}