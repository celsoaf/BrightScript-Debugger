using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class FunctionToken : Token
    {
        public FunctionToken(LexSpan lex) 
            : base(SyntaxKind.FunctionKeyword, lex)
        {
        }
    }
}