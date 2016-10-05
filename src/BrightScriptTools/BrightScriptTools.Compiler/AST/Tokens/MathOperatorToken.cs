namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class MathOperatorToken : OperatorToken
    {
        public MathOperatorToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
        }
    }
}