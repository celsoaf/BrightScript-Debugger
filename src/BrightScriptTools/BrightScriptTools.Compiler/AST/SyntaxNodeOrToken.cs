using System.Collections.Immutable;

namespace BrightScriptTools.Compiler.AST
{
    public abstract class SyntaxNodeOrToken
    {
        public virtual bool IsLeafNode => true;
        public virtual bool IsToken => true;
        public SyntaxKind Kind { get; protected set; }
        public virtual int Start { get; protected set; }
        public virtual int Length { get; protected set; }
        public int End => this.Start + this.Length;
        public abstract ImmutableList<SyntaxNodeOrToken> Children { get; }
    }
}