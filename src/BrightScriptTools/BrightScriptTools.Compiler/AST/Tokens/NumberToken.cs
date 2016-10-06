namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class NumberToken : LiteralToken
    {
        public NumberToken(LexSpan lex) 
            : base(SyntaxKind.Number, lex)
        {
            Value = decimal.Parse(lex.text);
        }

        public decimal Value { get; }
    }
}