namespace BrightScriptTools.Compiler.AST.Statements
{
    public class FunctionExpressionNode : StatementNode
    {
        public FunctionExpressionNode() 
            : base(SyntaxKind.FunctionDef)
        {
        }
    }
}