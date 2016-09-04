using System;

namespace BrightScriptTools.Compiler
{
    public partial class Scanner
    {
        public override void yyerror(string format, params object[] args)
        {
            base.yyerror(format, args);

            Console.WriteLine("Line {0} - Col {1} - {2}", tokLin, tokCol, format);
        }
    }

    public partial class ScannerColor
    {
        public int GetPos()
        {
            return yypos;
        }
    }
}