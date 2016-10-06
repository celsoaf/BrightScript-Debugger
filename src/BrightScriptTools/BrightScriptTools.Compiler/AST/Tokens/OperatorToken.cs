using System;
using System.Collections.Generic;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class OperatorToken : Token
    {
        private static readonly IDictionary<Compiler.Tokens, OperatorEnum> OperatorMap = new Dictionary<Compiler.Tokens, OperatorEnum>()
        {
            { Compiler.Tokens.equal, OperatorEnum.Equal },
            { Compiler.Tokens.notEqual, OperatorEnum.NotEqual },
            { Compiler.Tokens.lt, OperatorEnum.Less },
            { Compiler.Tokens.ltEqual, OperatorEnum.LessOrEqual },
            { Compiler.Tokens.gt, OperatorEnum.Greater },
            { Compiler.Tokens.gtEqual, OperatorEnum.GreaterOrEqual },
            { Compiler.Tokens.plus, OperatorEnum.Plus },
            { Compiler.Tokens.minus, OperatorEnum.Minus },
            { Compiler.Tokens.star, OperatorEnum.Star },
            { Compiler.Tokens.slash, OperatorEnum.Slash },
        };

        public OperatorToken(LexSpan lex) 
            : base(SyntaxKind.OperatorKeyword, lex)
        {
            Value = OperatorMap[(Compiler.Tokens)lex.token];
        }

        public OperatorEnum Value { get; }
    }
}