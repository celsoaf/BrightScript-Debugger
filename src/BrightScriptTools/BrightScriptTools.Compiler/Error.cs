namespace BrightScriptTools.Compiler
{
    public class Error
    {
        public Error(int position, int length, string message)
        {
            Position = position;
            Message = message;
            Length = length;
        }

        public Error(int line, int column, int position, int length, string message)
            : this(position, length, message)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; private set; }
        public int Column { get; private set; }
        public int Position { get; private set; }
        public int Length { get; private set; }
        public string Message { get; private set; }

    }
}