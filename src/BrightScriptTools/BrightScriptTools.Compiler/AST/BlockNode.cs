using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public class BlockNode : SyntaxNode
    {
        public BlockNode(SyntaxKind kind, BracketToken open, BracketToken close) 
            : base(kind, open.Start, close.End - open.Start)
        {
            list.Add(open);
            list.Add(close);
        }
    }
}