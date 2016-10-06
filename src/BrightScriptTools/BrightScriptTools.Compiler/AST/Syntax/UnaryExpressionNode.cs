namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class UnaryExpressionNode : SyntaxNode
    {
        public UnaryExpressionNode(Token token, SyntaxNode node) 
            : base(SyntaxKind.UnaryOperatorExpression)
        {
            AddNode(token);
            AddNode(node);
        }

        public UnaryExpressionNode(Token lPar, SyntaxNode node, Token rPar)
            : base(SyntaxKind.UnaryOperatorExpression)
        {
            AddNode(lPar);
            AddNode(node);
            AddNode(rPar);
        }
    }
}