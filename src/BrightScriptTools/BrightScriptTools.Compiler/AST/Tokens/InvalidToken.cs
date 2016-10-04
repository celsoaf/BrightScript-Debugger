namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InvalidToken : LiteralToken
    {
        public InvalidToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            
        }
    }
}