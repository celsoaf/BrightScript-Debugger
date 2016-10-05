using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class LiteralToken : Token
    {
        public LiteralToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}