using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class OrToken : Token
    {
        public OrToken(LexSpan lex) 
            : base(SyntaxKind.OrBinop, lex)
        {
        }
    }
}