using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ElseToken : Token
    {
        public ElseToken(LexSpan lex) 
            : base(SyntaxKind.ElseKeyword, lex)
        {
        }
    }
}