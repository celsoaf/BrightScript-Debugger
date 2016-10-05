using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class PrintToken : Token
    {
        public PrintToken(LexSpan lex) 
            : base(SyntaxKind.PrintToken, lex)
        {
        }
    }
}