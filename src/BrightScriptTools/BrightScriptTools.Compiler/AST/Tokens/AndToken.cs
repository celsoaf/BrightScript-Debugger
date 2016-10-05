using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AndToken : KeywordToken
    {
        public AndToken(LexSpan lex) 
            : base(SyntaxKind.AndBinop, lex)
        {
        }
    }
}