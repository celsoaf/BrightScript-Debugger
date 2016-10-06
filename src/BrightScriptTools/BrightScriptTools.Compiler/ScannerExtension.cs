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
                case Tokens.bsNumber: return SyntaxKind.Number;
                case Tokens.dot: return SyntaxKind.Dot;
                case Tokens.semi: return SyntaxKind.Semicolon;
                case Tokens.star: return SyntaxKind.MultiplyOperator;
                case Tokens.lt: return SyntaxKind.LessThanOperator;
                case Tokens.gt: return  SyntaxKind.GreaterThanOperator;
                case Tokens.ltEqual: return SyntaxKind.LessThanOrEqualOperator;
                case Tokens.gtEqual: return SyntaxKind.GreaterOrEqualThanOperator;
                case Tokens.notEqual: return  SyntaxKind.NotEqualOperator;
                case Tokens.comma: return SyntaxKind.Comma;
                case Tokens.slash: return SyntaxKind.DivideOperator;
                case Tokens.lBrac: return SyntaxKind.OpenBracket;
                case Tokens.rBrac: return SyntaxKind.CloseBracket;
                case Tokens.lPar: return SyntaxKind.OpenParen;
                case Tokens.lBrace: return SyntaxKind.OpenCurlyBrace;
                case Tokens.rBrace: return SyntaxKind.CloseCurlyBrace;
                case Tokens.Eol: return SyntaxKind.EolToken;
                case Tokens.equal: return SyntaxKind.EqualityOperator;
                case Tokens.plus: return SyntaxKind.PlusOperator;
                case Tokens.minus: return SyntaxKind.MinusOperator;
                case Tokens.questionMark: return SyntaxKind.PrintKeyword;
                case Tokens.colon: return SyntaxKind.Colon;
                case Tokens.bsIdent: return SyntaxKind.Identifier;
                case Tokens.bsStr: return SyntaxKind.String;
                case Tokens.bsFuncs: return SyntaxKind.GlobalFunctionToken;
                case Tokens.bsType: return SyntaxKind.Type;
                case Tokens.bsAs: return SyntaxKind.AsKeyword;
                case Tokens.bsTrue: return SyntaxKind.TrueKeyValue;
                case Tokens.bsFalse: return SyntaxKind.FalseKeyValue;
                case Tokens.bsInvalid: return SyntaxKind.InvalidKeyValue;
                case Tokens.bsNot: return SyntaxKind.NotUnop;
                case Tokens.bsAnd: return SyntaxKind.AndBinop;
                case Tokens.bsOr: return SyntaxKind. OrBinop;
                case Tokens.bsM: return SyntaxKind.Identifier;
                case Tokens.bsStop: return SyntaxKind.StopKeyword;
                case Tokens.bsPrint: return SyntaxKind.PrintKeyword;
                case Tokens.bsIf: return SyntaxKind.IfKeyword;
                case Tokens.bsElse: return SyntaxKind.ElseKeyword;
                case Tokens.bsFor: return SyntaxKind.ForKeyword;
                case Tokens.bsTo: return SyntaxKind.ToKeyword;
                case Tokens.bsEach: return SyntaxKind.EachKeyword;
                case Tokens.bsStep: return SyntaxKind.StepKeyword;
                case Tokens.bsIn: return SyntaxKind.InKeyword;
                case Tokens.bsWhile: return SyntaxKind.WhileKeyword;
                case Tokens.bsFunction: return SyntaxKind.FunctionKeyword;
                case Tokens.bsEnd: return SyntaxKind.EndKeyword;

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