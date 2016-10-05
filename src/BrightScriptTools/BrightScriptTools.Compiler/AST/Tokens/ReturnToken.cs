using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ReturnToken : KeywordToken
    {
        public ReturnToken(LexSpan lex) 
            : base(SyntaxKind.ReturnKeyword, lex)
        {
        }
    }
}