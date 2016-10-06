using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class SubToken : KeywordToken
    {
        public SubToken(LexSpan lex) 
            : base(SyntaxKind.SubKeyword, lex)
        {
        }
    }
}