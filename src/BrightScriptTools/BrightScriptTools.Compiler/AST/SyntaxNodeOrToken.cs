using System.Collections.Immutable;

namespace BrightScriptTools.Compiler.AST
{
    public abstract class SyntaxNodeOrToken
    {
        public virtual bool IsLeafNode => true;
        public virtual bool IsToken => true;
        public abstract ImmutableList<SyntaxNodeOrToken> Children { get; }
    }
}