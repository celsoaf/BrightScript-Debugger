using System;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class TypeToken : Token
    {
        public TypeToken(LexSpan lex) 
            : base(SyntaxKind.Type, lex)
        {
            Value = (TypeEnum)Enum.Parse(typeof(TypeEnum), lex.text);
        }

        public TypeEnum Value { get; }
    }
}