using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class ParenToken : Token
    {
        public ParenToken(LexSpan lex)
            : base(lex.token == (int)Compiler.Tokens.lPar ? SyntaxKind.OpenParen : SyntaxKind.CloseParen, lex)
        {
        }
    }
}