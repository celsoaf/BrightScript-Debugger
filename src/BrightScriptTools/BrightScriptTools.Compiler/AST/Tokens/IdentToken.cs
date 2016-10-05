namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class IdentToken : Token
    {
        public IdentToken(SyntaxKind kind, LexSpan lex) 
            : base(kind, lex)
        {
            Name = lex.text;
        }

        public string Name { get; }
    }
}