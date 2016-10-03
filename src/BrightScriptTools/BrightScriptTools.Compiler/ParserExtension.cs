using System.Collections.Generic;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.Compiler.AST.Tokens;
using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public partial class Parser : ShiftReduceParser<SyntaxNodeOrToken, LexSpan>
    {
        internal ErrorHandler handler;


        public Parser(AbstractScanner<SyntaxNodeOrToken, LexSpan> scanner, ErrorHandler handler)
            : base(scanner)
        {
            this.handler = handler;
        }

        public NumberNode BuildNumberNode(LexSpan lex)
        {
            return new NumberNode(SyntaxKind.Number, lex.text, lex.startIndex);
        }

        public StringNode BuildStringNode(LexSpan lex)
        {
            return new StringNode(SyntaxKind.String, lex.text, lex.startIndex);
        }

        public BooleanNode BuildBooleanNode(LexSpan lex, bool value)
        {
            return new BooleanNode(
                value ? SyntaxKind.TrueKeyValue : SyntaxKind.FalseKeyValue,
                lex.text, lex.startIndex);
        }

        public InvalidNode BuildInvalidNode(LexSpan lex)
        {
            return new InvalidNode(SyntaxKind.NilKeyValue, lex.text, lex.startIndex);
        }
    }
}