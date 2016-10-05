using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ToToken : Token
    {
        public ToToken(LexSpan lex) 
            : base(SyntaxKind.ToKeyword, lex)
        {
        }
    }
}