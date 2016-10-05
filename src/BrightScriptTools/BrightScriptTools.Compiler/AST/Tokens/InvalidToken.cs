namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InvalidToken : LiteralToken
    {
        public InvalidToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
            
        }
    }
}