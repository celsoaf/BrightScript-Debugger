namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class IdentToken : Token
    {
        public IdentToken(LexSpan lex) 
            : base(SyntaxKind.Identifier, lex)
        {
            Name = lex.text;
        }

        public string Name { get; }
    }
}