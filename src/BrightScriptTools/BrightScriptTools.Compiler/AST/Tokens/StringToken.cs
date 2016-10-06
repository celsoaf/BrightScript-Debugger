namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class StringToken : LiteralToken
    {
        public StringToken(LexSpan lex) 
            : base(SyntaxKind.String, lex)
        {
            Value = lex.text;
        }

        public string Value { get; }
    }
}