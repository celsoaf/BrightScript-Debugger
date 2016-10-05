using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterNode : SyntaxNode
    {
        public ParameterNode(SyntaxKind kind, IdentToken ident, TypeNode typeNode)
            : base(kind)
        {
            AddNode(ident);
            if (typeNode != null)
                AddNode(typeNode);
        }

        public ParameterNode(SyntaxKind kind, IdentToken ident, OperatorToken opr, LiteralNode literal, TypeNode typeNode)
            : base(kind)
        {
            AddNode(ident);
            AddNode(opr);
            AddNode(literal);
            AddNode(typeNode);
        }
    }
}