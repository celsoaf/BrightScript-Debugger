using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class MemberExpressionNode : SyntaxNode
    {
        public MemberExpressionNode(SyntaxKind kind, IdentToken ident) 
            : base(kind)
        {
            AddNode(ident);
        }
    }
}