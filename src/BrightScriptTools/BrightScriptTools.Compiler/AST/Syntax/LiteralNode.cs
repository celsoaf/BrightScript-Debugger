using System.Collections.Generic;
using System.Collections.Immutable;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class LiteralNode : SyntaxNode
    {
        public LiteralNode(SyntaxNodeOrToken token) : 
            base(SyntaxKind.LiteralNode)
        {
            AddNode(token);
        }
    }
}