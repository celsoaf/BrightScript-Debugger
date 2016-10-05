using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InToken : Token
    {
        public InToken(LexSpan lex) 
            : base(SyntaxKind.InKeyword, lex)
        {
        }
    }
}