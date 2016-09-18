using System.Collections.Generic;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public partial class Parser : ShiftReduceParser<SyntaxNodeOrToken, LexSpan>
    {
        internal ErrorHandler handler;


        public Parser(AbstractScanner<SyntaxNodeOrToken, LexSpan> scanner, ErrorHandler handler)
            :base(scanner)
        {
            this.handler = handler;
        }

        public NumberNode BuildNumberNode(object obj)
        {
            return new NumberNode(SyntaxKind.Number, ((Scanner)Scanner).GetText(), new List<Trivia>(), ((Scanner)Scanner).GetPosition(), ((Scanner)Scanner).GetPosition());
        }
    }
}