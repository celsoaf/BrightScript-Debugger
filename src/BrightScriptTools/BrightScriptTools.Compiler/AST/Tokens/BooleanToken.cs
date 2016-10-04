namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BooleanToken : LiteralToken
    {
        public BooleanToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = bool.Parse(text);
        }

        public bool Value { get; }
    }
}