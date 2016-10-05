using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class EndToken : Token
    {
        public EndToken(LexSpan lex) 
            : base(SyntaxKind.EndKeyword, lex)
        {
        }
    }
}