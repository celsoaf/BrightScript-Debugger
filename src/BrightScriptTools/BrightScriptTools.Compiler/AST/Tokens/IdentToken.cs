namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class IdentToken : Token
    {
        public IdentToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Name = text;
        }

        public string Name { get; }
    }
}