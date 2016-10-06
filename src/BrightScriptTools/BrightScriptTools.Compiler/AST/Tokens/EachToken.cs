using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class EachToken : KeywordToken
    {
        public EachToken(LexSpan lex) 
            : base(SyntaxKind.EachKeyword, lex)
        {
        }
    }
}