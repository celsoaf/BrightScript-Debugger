using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class SubToken : Token
    {
        public SubToken(LexSpan lex) 
            : base(SyntaxKind.SubKeyword, lex)
        {
        }
    }
}