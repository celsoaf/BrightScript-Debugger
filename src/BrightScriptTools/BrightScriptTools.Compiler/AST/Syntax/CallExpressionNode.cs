namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class CallExpressionNode : SyntaxNode
    {
        public CallExpressionNode() 
            : base(SyntaxKind.FunctionCallExp)
        {
        }
    }
}