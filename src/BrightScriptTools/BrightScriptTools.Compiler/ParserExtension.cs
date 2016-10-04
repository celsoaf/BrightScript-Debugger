using System.Collections.Generic;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.Compiler.AST.Syntax;
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

        public NumberToken BuildNumberNode(LexSpan lex)
        {
            return new NumberToken(SyntaxKind.Number, lex.text, lex.startIndex);
        }

        public StringToken BuildStringNode(LexSpan lex)
        {
            return new StringToken(SyntaxKind.String, lex.text, lex.startIndex);
        }

        public BooleanToken BuildBooleanNode(LexSpan lex, bool value)
        {
            return new BooleanToken(
                value ? SyntaxKind.TrueKeyValue : SyntaxKind.FalseKeyValue,
                lex.text, lex.startIndex);
        }

        public InvalidToken BuildInvalidNode(LexSpan lex)
        {
            return new InvalidToken(SyntaxKind.InvalidKeyValue, lex.text, lex.startIndex);
        }

        public BlockNode BuildEmptyBlock(LexSpan lexStart, LexSpan lexEnd)
        {
            return new BlockNode(SyntaxKind.BlockNode, 
                new BracketToken(SyntaxKind.OpenBracket, lexStart.text, lexStart.startIndex), 
                new BracketToken(SyntaxKind.CloseBracket, lexEnd.text, lexEnd.startIndex));
        }

        public TypeNode BuildTypeNode(LexSpan lexStart, LexSpan lexEnd)
        {
            return new TypeNode(
                SyntaxKind.TypeNode,
                new AsToken(SyntaxKind.AsKeyword, lexStart.text, lexStart.startIndex),
                new TypeToken(SyntaxKind.Type, lexEnd.text, lexEnd.startIndex)
                );
        }

        public ParameterNode BuildParameterNode(LexSpan ident, SyntaxNodeOrToken typeNode)
        {
            return  new ParameterNode(
                SyntaxKind.ParameterNode, 
                new IdentToken(SyntaxKind.Identifier, ident.text, ident.startIndex), 
                (TypeNode)typeNode
                );
        }

        public LiteralNode BuildLiteralNode(SyntaxNodeOrToken literalToken)
        {
            return new LiteralNode(SyntaxKind.LiteralNode, (LiteralToken)literalToken);
        }
    }
}