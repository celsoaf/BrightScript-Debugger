using System.Collections.Generic;
using System.Collections.Immutable;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class LiteralNode : SyntaxNode
    {
        public LiteralNode(SyntaxKind kind, SyntaxNodeOrToken token) : 
            base(kind)
        {
            list.Add(token);
        }
    }
}