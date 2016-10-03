using System.Collections.Immutable;
using System.Linq;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public partial class BlockNode : SyntaxNode
    {
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