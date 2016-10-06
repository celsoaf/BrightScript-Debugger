namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class LabelSeparatorNode : SyntaxNode
    {
        public LabelSeparatorNode(Token separator) 
            : base(SyntaxKind.LabelSeparatorNode)
        {
            AddNode(separator);
        }
    }
}