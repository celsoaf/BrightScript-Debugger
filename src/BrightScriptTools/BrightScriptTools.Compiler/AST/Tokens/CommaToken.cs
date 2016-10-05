using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class CommaToken : Token
    {
        public CommaToken(LexSpan lex) 
            : base(SyntaxKind.Comma, lex)
        {
        }
    }
}