using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AndToken : Token
    {
        public AndToken(LexSpan lex) 
            : base(SyntaxKind.AndBinop, lex)
        {
        }
    }
}