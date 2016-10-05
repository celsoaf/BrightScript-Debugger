using System.Collections.Generic;
using System.Collections.Immutable;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class TypeNode : SyntaxNode
    {
        public TypeNode(SyntaxKind kind, AsToken asToken, TypeToken typeToken)
            : base(kind)
        {
            AddNode(asToken);
            AddNode(typeToken);
        }
    }
}