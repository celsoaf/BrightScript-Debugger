namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class SingleExpressionNode : SyntaxNode
    {
        public SingleExpressionNode(SyntaxNode node) 
            : base(SyntaxKind.SimpleExpression)
        {
            AddNode(node);
        }
    }
}