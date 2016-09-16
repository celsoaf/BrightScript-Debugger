using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using BrightScriptTools.Gppg.GPGen.Parser;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public partial class Scanner
    {
        private ErrorHandler yyhdlr;
        public void SetHandler(ErrorHandler hdlr) { yyhdlr = hdlr; }

        public override void yyerror(string format, params object[] args)
        {
            if (yyhdlr != null)
            {
                LexSpan span = TokenSpan();
                if (args == null || args.Length == 0)
                    yyhdlr.AddError(2, format, span);
                else
                    yyhdlr.AddError(3, String.Format(CultureInfo.InvariantCulture, format, args), span);
            }
        }

        private LexSpan TokenSpan()
        {
            return new LexSpan(tokLin, tokCol, tokELin, tokECol, tokPos, tokEPos, buffer);
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