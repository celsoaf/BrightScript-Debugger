namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BraceToken : Token
    {
        public BraceToken(LexSpan lex)
            : base(lex.token == (int) Compiler.Tokens.lBrace ? SyntaxKind.OpenCurlyBrace : SyntaxKind.CloseCurlyBrace, lex)
        {
        }
    }
}