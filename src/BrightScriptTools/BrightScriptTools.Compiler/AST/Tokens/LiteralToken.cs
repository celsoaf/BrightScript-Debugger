using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class LiteralToken : Token
    {
        public LiteralToken(SyntaxKind kind, string text, List<Trivia> trivia, int fullStart, int start) 
            : base(kind, text, trivia, fullStart, start)
        {
        }
    }
}