using System;
using System.Collections.Generic;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class OperatorToken : Token
    {
        private static IDictionary<string, OperatorEnum> operatorMap = new Dictionary<string, OperatorEnum>()
        {
            { "=", OperatorEnum.Equal },
            { "<", OperatorEnum.Less },
            { ">", OperatorEnum.Greater },
            { "+", OperatorEnum.Plus },
            { "-", OperatorEnum.Minus },
            { "*", OperatorEnum.Star },
            { "/", OperatorEnum.Slash },
        };

        public OperatorToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = operatorMap[text];
        }

        public OperatorEnum Value { get; }
    }
}