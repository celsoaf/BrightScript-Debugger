using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class WhileToken : KeywordToken
    {
        public WhileToken(LexSpan lex) 
            : base(SyntaxKind.WhileKeyword, lex)
        {
        }
    }
}