using System.Collections.Immutable;
using System.Linq;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class BlockNode : SyntaxNode
    {
        [Required, NotRecursive]
        readonly ImmutableList<StatementNode> statements;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return statements.Cast<SyntaxNodeOrToken>().ToImmutableList();
            }
        }
    }
}