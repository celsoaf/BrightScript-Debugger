using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterNode : SyntaxNode
    {
        public ParameterNode(IdentToken ident, TypeNode typeNode)
            : base(SyntaxKind.ParameterNode)
        {
            AddNode(ident);
            if (typeNode != null)
                AddNode(typeNode);
        }

        public ParameterNode(IdentToken ident, OperatorToken opr, LiteralNode literal, TypeNode typeNode)
            : base(SyntaxKind.ParameterNode)
        {
            AddNode(ident);
            AddNode(opr);
            AddNode(literal);
            AddNode(typeNode);
        }
    }
}