namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InvalidToken : KeywordToken
    {
        public InvalidToken(LexSpan lex) 
            : base(SyntaxKind.InvalidKeyValue, lex)
        {
            
        }
    }
}