using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class CommaToken : Token
    {
        public CommaToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}