namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InvalidToken : LiteralToken
    {
        public InvalidToken(LexSpan lex) 
            : base(SyntaxKind.InvalidKeyValue, lex)
        {
            
        }
    }
}