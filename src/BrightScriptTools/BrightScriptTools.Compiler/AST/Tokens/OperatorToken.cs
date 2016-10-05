using System;
using System.Collections.Generic;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class OperatorToken : Token
    {
        private static readonly IDictionary<string, OperatorEnum> OperatorMap = new Dictionary<string, OperatorEnum>()
        {
            { "=", OperatorEnum.Equal },
            { "<>", OperatorEnum.NotEqual },
            { "<", OperatorEnum.Less },
            { "<=", OperatorEnum.LessOrEqual },
            { ">", OperatorEnum.Greater },
            { ">=", OperatorEnum.GreaterOrEqual },
            { "+", OperatorEnum.Plus },
            { "-", OperatorEnum.Minus },
            { "*", OperatorEnum.Star },
            { "/", OperatorEnum.Slash },
        };

        public OperatorToken(LexSpan lex) 
            : base(SyntaxKind.OperatorKeyword, lex)
        {
            Value = OperatorMap[lex.text];
        }

        public OperatorEnum Value { get; }
    }
}