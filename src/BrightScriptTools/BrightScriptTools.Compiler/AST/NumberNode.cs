using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST
{
    public class NumberNode : Token
    {
        public NumberNode(SyntaxKind kind, string text, List<Trivia> trivia, int fullStart, int start) 
            : base(kind, text, trivia, fullStart, start)
        {
        }
    }
}