using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class GlobalFunctionToken : KeywordToken
    {
        public GlobalFunctionToken(LexSpan lex) 
            : base(SyntaxKind.GlobalFunctionToken, lex)
        {
            FuncName = lex.text;
        }

        public string FuncName { get; set; }
    }
}