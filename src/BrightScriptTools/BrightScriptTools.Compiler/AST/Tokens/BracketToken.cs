using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BracketToken : Token
    {
        public BracketToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
        }
    }
}