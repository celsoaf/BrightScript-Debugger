using System;

namespace BrightScriptDebug.Compiler
{
    public partial class Scanner
    {
        public event Action<string> ErrorPorcessed;

        //public bool Restart { get; set; }

        public override void yyerror(string format, params object[] args)
        {
            base.yyerror(format, args);

            var msg = $"Line {tokLin} - Col {tokCol} - {format}";

            Console.WriteLine(msg);

            ErrorPorcessed?.Invoke(msg);

            //if (tokELin > 1000)
            //    Restart = true;
        }
    }
}