using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ForToken : Token
    {
        public ForToken(LexSpan lex) 
            : base(SyntaxKind.ForKeyword, lex)
        {
        }
    }
}