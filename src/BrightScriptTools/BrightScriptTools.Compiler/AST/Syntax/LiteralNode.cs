using System.Collections.Generic;
using System.Collections.Immutable;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class LiteralNode : SyntaxNode
    {
        public LiteralNode(SyntaxKind kind, LiteralToken token) : 
            base(kind, token.Start, token.Length)
        {
            list.Add(token);
        }
    }
}