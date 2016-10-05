using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class OrToken : KeywordToken
    {
        public OrToken(LexSpan lex) 
            : base(SyntaxKind.OrBinop, lex)
        {
        }
    }
}