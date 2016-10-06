using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class DotToken : Token
    {
        public DotToken(LexSpan lex) 
            : base(SyntaxKind.Dot, lex)
        {
        }
    }
}