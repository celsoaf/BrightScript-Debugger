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
            return new NumberToken(SyntaxKind.Number, lex);
        }

        public StringToken BuildStringNode(LexSpan lex)
        {
            return new StringToken(SyntaxKind.String, lex);
        }

        public BooleanToken BuildBooleanNode(LexSpan lex, bool value)
        {
            return new BooleanToken(
                value ? SyntaxKind.TrueKeyValue : SyntaxKind.FalseKeyValue,
                lex);
        }

        public InvalidToken BuildInvalidNode(LexSpan lex)
        {
            return new InvalidToken(SyntaxKind.InvalidKeyValue, lex);
        }

        public BlockNode BuildEmptyBlock(LexSpan lexStart, LexSpan lexEnd)
        {
            return new BlockNode(SyntaxKind.BlockNode, 
                new BracketToken(SyntaxKind.OpenBracket, lexStart), 
                new BracketToken(SyntaxKind.CloseBracket, lexEnd));
        }

        public TypeNode BuildTypeNode(LexSpan lexStart, LexSpan lexEnd)
        {
            return new TypeNode(
                SyntaxKind.TypeNode,
                new AsToken(SyntaxKind.AsKeyword, lexStart),
                new TypeToken(SyntaxKind.Type, lexEnd)
                );
        }

        public ParameterNode BuildParameterNode(LexSpan ident, SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(
                SyntaxKind.ParameterNode,
                new IdentToken(SyntaxKind.Identifier, ident),
                (TypeNode)typeNode
                );
        }

        public ParameterNode BuildParameterNode(LexSpan ident, LexSpan equal, SyntaxNodeOrToken literal, SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(
                SyntaxKind.ParameterNode,
                new IdentToken(SyntaxKind.Identifier, ident),
                new OperatorToken(SyntaxKind.OperatorKeyword, equal),
                (LiteralNode)literal,
                (TypeNode)typeNode
                );
        }

        public LiteralNode BuildLiteralNode(SyntaxNodeOrToken literalToken)
        {
            return new LiteralNode(SyntaxKind.LiteralNode, literalToken);
        }

        public MathOperatorToken BuildMathOperatorNode(LexSpan lex)
        {
            return new MathOperatorToken(SyntaxKind.OperatorKeyword, lex);
        }

        public BooleanOperatorToken BuildBooleanOperatorNode(LexSpan lex)
        {
            return new BooleanOperatorToken(SyntaxKind.OperatorKeyword, lex);
        }
    }
}