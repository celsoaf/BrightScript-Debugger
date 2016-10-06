using System.Linq;

namespace BrightScriptTools.Compiler.AST
{
    public class SyntaxListNode : SyntaxNode
    {
        public SyntaxListNode(SyntaxKind kind) : base(kind)
        {
        }

        internal override void AddNode(SyntaxNodeOrToken elem)
        {
            base.AddNode(elem);
            list = list.OrderBy(e => e.Start)
                        .ToList();
        }
    }
}