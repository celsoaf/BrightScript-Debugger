namespace BrightScriptTools.Compiler.AST.Statements
{
    public class RootNode : SyntaxNode
    {
        public RootNode()
            : base(SyntaxKind.ProgramNode)
        {
        }
    }
}