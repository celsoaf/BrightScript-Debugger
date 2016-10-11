using System;
using BrightScriptTools.Compiler.AST.Enums;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class TypeToken : Token
    {
        public TypeToken(LexSpan lex) 
            : base(SyntaxKind.Type, lex)
        {
            TypeEnum t;
            if (Enum.TryParse(lex.text, true, out t))
                Value = t;
        }

        public TypeEnum Value { get; }
    }
}