using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class MemberExpressionNode : SyntaxNode
    {
        public MemberExpressionNode(IdentToken ident) 
            : base(SyntaxKind.MemberExpressionNode)
        {
            AddNode(ident);
        }
    }
}