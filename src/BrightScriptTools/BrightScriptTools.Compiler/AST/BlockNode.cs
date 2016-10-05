using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public class BlockNode : SyntaxListNode
    {
        public BlockNode() 
            : base(SyntaxKind.BlockNode)
        {
        }
    }
}