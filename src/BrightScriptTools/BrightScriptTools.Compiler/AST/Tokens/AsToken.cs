namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AsToken : KeywordToken
    {
        public AsToken(LexSpan lex) 
            : base(SyntaxKind.AsKeyword, lex)
        {
        }
    }
}