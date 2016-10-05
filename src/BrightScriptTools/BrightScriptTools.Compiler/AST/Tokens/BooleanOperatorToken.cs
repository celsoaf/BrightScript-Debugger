namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BooleanOperatorToken : OperatorToken
    {
        public BooleanOperatorToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}