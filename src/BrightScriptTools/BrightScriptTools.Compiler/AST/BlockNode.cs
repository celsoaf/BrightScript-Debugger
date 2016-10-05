using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public class BlockNode : SyntaxNode
    {
        public BlockNode() 
            : base(SyntaxKind.BlockNode)
        {
        }
    }
}