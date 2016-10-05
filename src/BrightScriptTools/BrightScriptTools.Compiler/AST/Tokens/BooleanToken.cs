namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BooleanToken : LiteralToken
    {
        public BooleanToken(LexSpan lex, bool value) 
            : base(value ? SyntaxKind.TrueKeyValue : SyntaxKind.FalseKeyValue,  lex)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}