namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class UnaryExpressionNode : SyntaxNode
    {
        public UnaryExpressionNode(Token token, SyntaxNode node) 
            : base(SyntaxKind.UnaryExpressionNode)
        {
            AddNode(token);
            AddNode(node);
        }

        public UnaryExpressionNode(Token lPar, SyntaxNode node, Token rPar)
            : base(SyntaxKind.UnaryExpressionNode)
        {
            AddNode(lPar);
            AddNode(node);
            AddNode(rPar);
        }
    }
}