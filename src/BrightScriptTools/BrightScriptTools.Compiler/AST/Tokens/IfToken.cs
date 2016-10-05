﻿using System.Collections.Generic;

namespace BrightScriptTools.Compiler.AST.Tokens
{
    public class IfToken : Token
    {
        public IfToken(LexSpan lex) 
            : base(SyntaxKind.IfKeyword, lex)
        {
        }
    }
}