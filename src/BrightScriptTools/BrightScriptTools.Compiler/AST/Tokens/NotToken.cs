using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class NotToken : Token
    {
        public NotToken(LexSpan lex) 
            : base(SyntaxKind.NotUnop, lex)
        {
        }
    }
}