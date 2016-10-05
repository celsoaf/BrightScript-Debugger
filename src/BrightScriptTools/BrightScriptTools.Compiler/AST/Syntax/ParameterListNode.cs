using System.Linq;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterListNode : SyntaxNode
    {
        public ParameterListNode(SyntaxKind kind) 
            : base(kind)
        {
        }

        public void AddNode(SyntaxNodeOrToken elem)
        {
            list.Add(elem);
            list = list
                .OrderBy(e => e.Start)
                .ToList();
        }
    }
}