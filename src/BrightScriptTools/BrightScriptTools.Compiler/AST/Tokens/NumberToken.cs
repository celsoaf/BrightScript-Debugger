namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class NumberToken : LiteralToken
    {
        public NumberToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
            Value = decimal.Parse(lex.text);
        }

        public decimal Value { get; }
    }
}