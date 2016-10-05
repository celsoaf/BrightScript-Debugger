namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class SingleExpressionNode : SyntaxNode
    {
        public SingleExpressionNode(SyntaxKind kind, SyntaxNode node) 
            : base(kind)
        {
            AddNode(node);
        }
    }
}