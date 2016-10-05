using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class EolToken : Token
    {
        public EolToken(LexSpan lex) 
            : base(SyntaxKind.EolToken, lex)
        {
        }
    }
}