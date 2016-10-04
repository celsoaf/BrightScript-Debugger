namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class AsToken : Token
    {
        public AsToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
        }
    }
}