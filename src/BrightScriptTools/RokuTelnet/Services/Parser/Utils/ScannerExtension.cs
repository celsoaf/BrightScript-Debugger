using System;

namespace BrightScriptDebug.Compiler
{
    public partial class Scanner
    {
        public event Action ErrorPorcessed;

        public override void yyerror(string format, params object[] args)
        {
            base.yyerror(format, args);

            Console.WriteLine("Line {0} - Col {1} - {2}", tokLin, tokCol, format);

            ErrorPorcessed?.Invoke();
        }
    }
}