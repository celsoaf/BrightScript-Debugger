namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class SingleExpressionNode : SyntaxNode
    {
        public SingleExpressionNode(SyntaxNode node) 
            : base(SyntaxKind.SingleExpressionNode)
        {
            AddNode(node);
        }
    }
}