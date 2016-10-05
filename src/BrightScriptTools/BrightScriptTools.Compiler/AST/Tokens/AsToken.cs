namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AsToken : Token
    {
        public AsToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}