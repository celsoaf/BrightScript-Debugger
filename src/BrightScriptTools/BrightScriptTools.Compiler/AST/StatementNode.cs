using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    [GenerateImmutable(GenerateBuilder = true)]
    public abstract partial class StatementNode : SyntaxNode { }
}