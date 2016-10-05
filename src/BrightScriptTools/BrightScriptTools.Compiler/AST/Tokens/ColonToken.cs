using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ColonToken : Token
    {
        public ColonToken(LexSpan lex) 
            : base(SyntaxKind.Colon, lex)
        {
        }
    }
}