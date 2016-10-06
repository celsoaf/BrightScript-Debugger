using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BracketToken : Token
    {
        public BracketToken(LexSpan lex) 
            : base(lex.token == (int)Compiler.Tokens.lBrac ? SyntaxKind.OpenBracket : SyntaxKind.CloseBracket, lex)
        {
        }
    }
}