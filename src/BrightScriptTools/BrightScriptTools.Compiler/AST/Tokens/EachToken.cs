using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class EachToken : Token
    {
        public EachToken(LexSpan lex) 
            : base(SyntaxKind.EachKeyword, lex)
        {
        }
    }
}