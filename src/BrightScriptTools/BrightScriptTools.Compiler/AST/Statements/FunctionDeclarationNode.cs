namespace BrightScriptTools.Compiler.AST.Statements
{
    public class FunctionDeclarationNode : StatementNode
    {
        public FunctionDeclarationNode() 
            : base(SyntaxKind.FunctionStatementDef)
        {
        }
    }
}