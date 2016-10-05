using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImmutableObjectGraph.Generation;

namespace BrightScriptTools.Compiler.AST
{
    public abstract partial class SyntaxNode : SyntaxNodeOrToken
    {
        readonly SyntaxKind kind;
        private IList<SyntaxNodeOrToken> list = new List<SyntaxNodeOrToken>();

        public SyntaxNode(SyntaxKind kind)
        {
            this.kind = kind;
        }

        public override bool IsToken => false;
        public override bool IsLeafNode => this.Children.Count == 0;

        public override int Start
        {
            get
            {
                if (list.Count > 0)
                    return list.First().Start;
                return 0;
            }
            protected set { throw new NotImplementedException(); }
        }

        public override int Length
        {
            get
            {
                if (list.Count > 0)
                    return list.Last().End - list.First().Start;
                return 0;
            }
            protected set { throw new NotImplementedException(); }
        }

        internal void AddNode(SyntaxNodeOrToken elem)
        {
            if (elem != null)
            {
                list.Add(elem);
                list = list
                    .OrderBy(e => e.Start)
                    .ToList();
            }
        }

        public override ImmutableList<SyntaxNodeOrToken> Children => list.ToImmutableList();

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