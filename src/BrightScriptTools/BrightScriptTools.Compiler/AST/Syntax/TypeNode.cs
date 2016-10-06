using System.Collections.Generic;
using System.Collections.Immutable;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class TypeNode : SyntaxNode
    {
        public TypeNode(AsToken asToken, TypeToken typeToken)
            : base(SyntaxKind.TypeNode)
        {
            AddNode(asToken);
            AddNode(typeToken);
        }
    }
}