namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class NumberToken : LiteralToken
    {
        public NumberToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = decimal.Parse(text);
        }

        public decimal Value { get; }
    }
}