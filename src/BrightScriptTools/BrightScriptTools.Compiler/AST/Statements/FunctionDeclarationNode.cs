namespace BrightScriptTools.Compiler.AST.Statements
{
    public class FunctionDeclarationNode : SourceElementNode
    {
        public FunctionDeclarationNode() 
            : base(SyntaxKind.FunctionStatementDef)
        {
        }
    }
}