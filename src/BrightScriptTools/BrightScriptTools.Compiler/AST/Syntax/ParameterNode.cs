﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler.AST.Syntax
{
    public class ParameterNode : SyntaxNode
    {
        public ParameterNode(SyntaxKind kind, IdentToken ident, TypeNode typeNode)
            : base(kind, ident.Start, typeNode.End - ident.Start)
        {
            list.Add(ident);
            list.Add(typeNode);
        }
    }
}