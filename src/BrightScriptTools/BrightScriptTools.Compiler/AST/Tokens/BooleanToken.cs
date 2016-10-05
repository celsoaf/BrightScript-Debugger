namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BooleanToken : LiteralToken
    {
        public BooleanToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
            Value = bool.Parse(lex.text);
        }

        public bool Value { get; }
    }
}