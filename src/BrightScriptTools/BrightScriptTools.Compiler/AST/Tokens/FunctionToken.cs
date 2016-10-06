using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class FunctionToken : KeywordToken
    {
        public FunctionToken(LexSpan lex) 
            : base(SyntaxKind.FunctionKeyword, lex)
        {
        }
    }
}