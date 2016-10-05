using System.Linq;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterListNode : SyntaxNode
    {
        public ParameterListNode(SyntaxKind kind) 
            : base(kind)
        {
        }
    }
}