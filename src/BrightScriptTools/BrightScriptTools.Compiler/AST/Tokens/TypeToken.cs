using System;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class TypeToken : Token
    {
        public TypeToken(SyntaxKind kind, string text, int start) 
            : base(kind, text, null, start, start)
        {
            Value = (TypeEnum)Enum.Parse(typeof(TypeEnum), text);
        }

        public TypeEnum Value { get; }
    }
}