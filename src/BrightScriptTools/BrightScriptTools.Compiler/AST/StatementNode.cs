using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public abstract partial class StatementNode : SyntaxNode {
        public StatementNode(SyntaxKind kind) 
            : base(kind)
        {
        }
    }
}