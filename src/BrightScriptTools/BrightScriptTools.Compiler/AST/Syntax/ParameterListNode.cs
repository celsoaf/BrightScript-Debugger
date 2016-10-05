using System.Linq;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterListNode : SyntaxListNode
    {
        public ParameterListNode() 
            : base(SyntaxKind.ParameterListNode)
        {
        }
    }
}