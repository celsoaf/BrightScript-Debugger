namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class StringToken : LiteralToken
    {
        public StringToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
            Value = lex.text;
        }

        public string Value { get; }
    }
}