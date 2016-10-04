using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public abstract partial class StatementNode : SyntaxNode {
        public StatementNode(SyntaxKind kind, int startPosition, int length) 
            : base(kind, startPosition, length)
        {
        }
    }
}