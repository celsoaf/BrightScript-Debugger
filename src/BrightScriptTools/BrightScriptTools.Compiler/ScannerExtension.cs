using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BrightScriptTools.Compiler
{
    public partial class Scanner
    {
        private List<Error> _errors = new List<Error>();
        public IEnumerable<Error> Errors { get { return _errors; } }

        public override void yyerror(string format, params object[] args)
        {
            base.yyerror(format, args);

            _errors.Add(new Error(tokPos, yyleng, string.Format(format, args)));

            Debug.WriteLine("Line {0} - Col {1} - {2}", tokLin, tokCol, format);
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