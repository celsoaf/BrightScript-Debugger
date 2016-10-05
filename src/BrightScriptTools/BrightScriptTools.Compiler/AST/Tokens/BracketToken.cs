using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BracketToken : Token
    {
        public BracketToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}