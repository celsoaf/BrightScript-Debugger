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

        private NumberToken BuildNumberNode(LexSpan lex)
        {
            return new NumberToken(SyntaxKind.Number, lex);
        }

        private StringToken BuildStringNode(LexSpan lex)
        {
            return new StringToken(SyntaxKind.String, lex);
        }

        private BooleanToken BuildBooleanNode(LexSpan lex, bool value)
        {
            return new BooleanToken(
                value ? SyntaxKind.TrueKeyValue : SyntaxKind.FalseKeyValue,
                lex);
        }

        private InvalidToken BuildInvalidNode(LexSpan lex)
        {
            return new InvalidToken(SyntaxKind.InvalidKeyValue, lex);
        }

        private BlockNode BuildEmptyBlock(LexSpan lexStart, LexSpan lexEnd)
        {
            return new BlockNode(SyntaxKind.BlockNode,
                new BracketToken(SyntaxKind.OpenBracket, lexStart),
                new BracketToken(SyntaxKind.CloseBracket, lexEnd));
        }

        private TypeNode BuildTypeNode(LexSpan lexStart, LexSpan lexEnd)
        {
            return new TypeNode(
                SyntaxKind.TypeNode,
                new AsToken(SyntaxKind.AsKeyword, lexStart),
                new TypeToken(SyntaxKind.Type, lexEnd)
                );
        }

        private ParameterNode BuildParameterNode(LexSpan ident, SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(
                SyntaxKind.ParameterNode,
                new IdentToken(SyntaxKind.Identifier, ident),
                (TypeNode)typeNode
                );
        }

        private ParameterNode BuildParameterNode(LexSpan ident, LexSpan equal, SyntaxNodeOrToken literal, SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(
                SyntaxKind.ParameterNode,
                new IdentToken(SyntaxKind.Identifier, ident),
                new OperatorToken(SyntaxKind.OperatorKeyword, equal),
                (LiteralNode)literal,
                (TypeNode)typeNode
                );
        }

        private ParameterListNode BuildParameterListNode()
        {
            return new ParameterListNode(SyntaxKind.ParameterListNode);
        }

        private ParameterListNode BuildParameterListNode(SyntaxNodeOrToken parameter, SyntaxNodeOrToken list)
        {
            var tail = list as ParameterListNode ?? new ParameterListNode(SyntaxKind.ParameterListNode);

            tail.AddNode(parameter);

            return tail;
        }

        private ParameterListNode BuildParameterListNode(LexSpan lex, SyntaxNodeOrToken parameter, SyntaxNodeOrToken list)
        {
            var tail = list as ParameterListNode ?? new ParameterListNode(SyntaxKind.ParameterListNode);
            tail.AddNode(new CommaToken(SyntaxKind.Comma, lex));
            tail.AddNode(parameter);

            return tail;
        }

        private LiteralNode BuildLiteralNode(SyntaxNodeOrToken literalToken)
        {
            return new LiteralNode(SyntaxKind.LiteralNode, literalToken);
        }

        private MathOperatorToken BuildMathOperatorNode(LexSpan lex)
        {
            return new MathOperatorToken(SyntaxKind.OperatorKeyword, lex);
        }

        private BooleanOperatorToken BuildBooleanOperatorNode(LexSpan lex)
        {
            return new BooleanOperatorToken(SyntaxKind.OperatorKeyword, lex);
        }

        public SingleExpressionNode BuildSingleExpressionNode(SyntaxNodeOrToken node)
        {
            return new SingleExpressionNode(SyntaxKind.SingleExpressionNode, (SyntaxNode)node);
        }

        public MemberExpressionNode BuildMemberExpressionNode(LexSpan lex)
        {
            return new MemberExpressionNode(
                SyntaxKind.MemberExpressionNode,
                new IdentToken(SyntaxKind.Identifier, lex));
        }

        public UnaryExpressionNode BuildUnaryExpressionNode(LexSpan lex, SyntaxNodeOrToken node)
        {
            if(lex.token == (int)Tokens.minus)
                return new UnaryExpressionNode(new MathOperatorToken(SyntaxKind.OperatorKeyword, lex), (SyntaxNode)node);

            if(lex.token == (int)Tokens.bsNot)
                return new UnaryExpressionNode(new NotToken(lex), (SyntaxNode)node);

            return null;
        }

        public UnaryExpressionNode BuildUnaryExpressionNode(LexSpan lPar, SyntaxNodeOrToken node, LexSpan rPar)
        {
            return new UnaryExpressionNode(new ParenToken(lPar), (SyntaxNode)node, new ParenToken(rPar));
        }
    }
}