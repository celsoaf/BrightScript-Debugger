using System.Collections.Generic;
using System.Collections.Immutable;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public abstract partial class SyntaxNode : SyntaxNodeOrToken
    {
        readonly SyntaxKind kind;
        readonly int startPosition;
        readonly int length;
        public override bool IsToken => false;
        public override bool IsLeafNode => this.Children.Count == 0;

        public IEnumerable<SyntaxNodeOrToken> Descendants()
        {
            if (this is SyntaxNode && ((SyntaxNode)this).Kind == SyntaxKind.ChunkNode)
            {
                yield return this;
            }

            foreach (var node in this.Children)
            {
                yield return node;

                var nodeAsSyntaxNode = node as SyntaxNode;

                if (nodeAsSyntaxNode != null)
                {
                    foreach (var nextNode in nodeAsSyntaxNode.Descendants())
                    {
                        yield return nextNode;
                    }
                }
            }
        }

        public SyntaxKind Kind
        {
            get
            {
                return this.kind;
            }
        }
    }
}