using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.Gppg.GPGen.Parser;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public partial class Scanner
    {
        private ErrorHandler yyhdlr;
        private LexSpan _yylloc;
        public void SetHandler(ErrorHandler hdlr) { yyhdlr = hdlr; }

        public override void yyerror(string format, params object[] args)
        {
            if (yyhdlr != null)
            {
                if (args == null || args.Length == 0)
                    yyhdlr.AddError(2, format, yylloc);
                else
                    yyhdlr.AddError(3, String.Format(CultureInfo.InvariantCulture, format, args), yylloc);
            }
        }

        protected LexSpan GetTokenSpan(int token)
        {
            return new LexSpan(tokLin, tokCol, tokELin, tokECol, tokPos, tokEPos, buffer) { text = yytext, token = token };
        }

        public BrightScriptTools.Compiler.AST.Token GetToken(int token)
        {
            return new Token(GetSyntaxkind(token), GetTokenSpan(token));
        }

        private SyntaxKind GetSyntaxkind(int token)
        {
            var t = (Tokens)token;

            switch (t)
            {
                case Tokens.bsNumber:
                    return SyntaxKind.Number;

                default:
                    return SyntaxKind.MissingToken;
            }
        }

        public string GetText()
        {
            return yytext;
        }

        public int GetPosition()
        {
            return tokPos;
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