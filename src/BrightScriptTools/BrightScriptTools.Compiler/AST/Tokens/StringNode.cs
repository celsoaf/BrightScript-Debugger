namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class StringNode : Token
    {
        public StringNode(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = text;
        }

        public string Value { get; }
    }
}