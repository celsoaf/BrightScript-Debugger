using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class StopToken : KeywordToken
    {
        public StopToken(LexSpan lex) 
            : base(SyntaxKind.StopKeyword, lex)
        {
        }
    }
}