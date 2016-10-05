using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class WhileToken : Token
    {
        public WhileToken(LexSpan lex) 
            : base(SyntaxKind.WhileKeyword, lex)
        {
        }
    }
}