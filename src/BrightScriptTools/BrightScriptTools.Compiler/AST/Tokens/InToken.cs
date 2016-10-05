using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InToken : KeywordToken
    {
        public InToken(LexSpan lex) 
            : base(SyntaxKind.InKeyword, lex)
        {
        }
    }
}