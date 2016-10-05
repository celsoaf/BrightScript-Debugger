namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AsToken : Token
    {
        public AsToken(LexSpan lex) 
            : base(SyntaxKind.AsKeyword, lex)
        {
        }
    }
}