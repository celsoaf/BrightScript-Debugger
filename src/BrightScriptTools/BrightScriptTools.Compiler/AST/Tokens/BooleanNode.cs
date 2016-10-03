namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class BooleanNode : Token
    {
        public BooleanNode(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = bool.Parse(text);
        }

        public bool Value { get; }
    }
}