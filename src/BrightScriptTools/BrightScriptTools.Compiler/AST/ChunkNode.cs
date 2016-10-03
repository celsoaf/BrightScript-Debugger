using System.Collections.Immutable;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    [GenerateImmutable(GenerateBuilder = true)]
    public partial class ChunkNode : SyntaxNode
    {
        readonly BlockNode programBlock;
        readonly Token endOfFile;

        public override ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return ImmutableList.Create<SyntaxNodeOrToken>(programBlock, endOfFile);
            }
        }
    }
}