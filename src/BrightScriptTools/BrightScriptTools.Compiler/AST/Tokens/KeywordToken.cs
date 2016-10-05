using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public abstract class KeywordToken : Token
    {
        public KeywordToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}