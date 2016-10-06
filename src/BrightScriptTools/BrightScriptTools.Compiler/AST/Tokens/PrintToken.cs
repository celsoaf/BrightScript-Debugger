using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class PrintToken : KeywordToken
    {
        public PrintToken(LexSpan lex) 
            : base(SyntaxKind.PrintKeyword, lex)
        {
        }
    }
}