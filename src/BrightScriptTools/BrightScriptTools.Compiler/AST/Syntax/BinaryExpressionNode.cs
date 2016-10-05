namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class BinaryExpressionNode : SyntaxNode
    {
        public BinaryExpressionNode() 
            : base(SyntaxKind.BinaryOperatorExpression)
        {
        }
    }
}