using System.Collections.Immutable;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
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

        public ChunkNode(SyntaxKind kind, int startPosition, int length) 
            : base(kind)
        {
            Start = startPosition;
            Length = length;
        }

        public override int Start { get; protected set; }
        public override int Length { get; protected set; }
    }
}