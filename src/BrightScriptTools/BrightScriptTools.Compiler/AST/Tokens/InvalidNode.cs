namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class InvalidNode : Token
    {
        public InvalidNode(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            
        }
    }
}