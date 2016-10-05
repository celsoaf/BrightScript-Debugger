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
            list.Add(ident);
            if (typeNode != null)
                list.Add(typeNode);
        }

        public ParameterNode(SyntaxKind kind, IdentToken ident, OperatorToken opr, LiteralNode literal, TypeNode typeNode)
            : base(kind)
        {
            list.Add(ident);
            list.Add(opr);
            list.Add(literal);
            list.Add(typeNode);
        }
    }
}