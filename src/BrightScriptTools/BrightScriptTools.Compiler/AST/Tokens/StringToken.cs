namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class StringToken : LiteralToken
    {
        public StringToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = text;
        }

        public string Value { get; }
    }
}