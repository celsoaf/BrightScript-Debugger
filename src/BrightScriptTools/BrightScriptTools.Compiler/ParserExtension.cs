using System;
using System.Collections.Generic;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.Compiler.AST.Statements;
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
            return new NumberToken(lex);
        }

        private StringToken BuildStringNode(LexSpan lex)
        {
            return new StringToken(lex);
        }

        private BooleanToken BuildBooleanNode(LexSpan lex, bool value)
        {
            return new BooleanToken(lex, value);
        }

        private InvalidToken BuildInvalidNode(LexSpan lex)
        {
            return new InvalidToken(lex);
        }

        private BlockNode BuildEmptyBlock(LexSpan lexStart, LexSpan lexEnd)
        {
            var exp = new BlockNode();
            exp.AddNode(new BraceToken(lexStart));
            exp.AddNode(new BraceToken(lexEnd));

            return exp;
        }

        private TypeNode BuildTypeNode(LexSpan lexAs, LexSpan lexType)
        {
            return new TypeNode(new AsToken(lexAs), new TypeToken(lexType)
                );
        }

        private ParameterNode BuildParameterNode(LexSpan ident, SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(new IdentToken(ident), (TypeNode)typeNode);
        }

        private ParameterNode BuildParameterNode(
            LexSpan ident, 
            LexSpan equal, 
            SyntaxNodeOrToken literal, 
            SyntaxNodeOrToken typeNode)
        {
            return new ParameterNode(
                new IdentToken(ident),
                new OperatorToken(equal),
                (LiteralNode)literal,
                (TypeNode)typeNode);
        }

        private ParameterListNode BuildParameterListNode()
        {
            return new ParameterListNode();
        }

        private ParameterListNode BuildParameterListNode(SyntaxNodeOrToken parameter, SyntaxNodeOrToken list)
        {
            var tail = list as ParameterListNode ?? new ParameterListNode();

            tail.AddNode(parameter);

            return tail;
        }

        private ParameterListNode BuildParameterListNode(LexSpan lex, SyntaxNodeOrToken parameter, SyntaxNodeOrToken list)
        {
            var tail = list as ParameterListNode ?? new ParameterListNode();
            tail.AddNode(new CommaToken(lex));
            tail.AddNode(parameter);

            return tail;
        }

        private LiteralNode BuildLiteralNode(SyntaxNodeOrToken literalToken)
        {
            return new LiteralNode(literalToken);
        }

        private MathOperatorToken BuildMathOperatorNode(LexSpan lex)
        {
            return new MathOperatorToken(lex);
        }

        private BooleanOperatorToken BuildBooleanOperatorNode(LexSpan lex)
        {
            return new BooleanOperatorToken(lex);
        }

        private SingleExpressionNode BuildSingleExpressionNode(SyntaxNodeOrToken node)
        {
            return new SingleExpressionNode((SyntaxNode)node);
        }

        private MemberExpressionNode BuildMemberExpressionNode(LexSpan lex)
        {
            return new MemberExpressionNode(new IdentToken(lex));
        }

        private UnaryExpressionNode BuildUnaryExpressionNode(LexSpan lex, SyntaxNodeOrToken node)
        {
            if (lex.token == (int)Tokens.minus)
                return new UnaryExpressionNode(new MathOperatorToken(lex), (SyntaxNode)node);

            if (lex.token == (int)Tokens.bsNot)
                return new UnaryExpressionNode(new NotToken(lex), (SyntaxNode)node);

            return null;
        }

        private UnaryExpressionNode BuildUnaryExpressionNode(LexSpan lPar, SyntaxNodeOrToken node, LexSpan rPar)
        {
            return new UnaryExpressionNode(new ParenToken(lPar), (SyntaxNode)node, new ParenToken(rPar));
        }

        private SequenceExpressionNode BuildSequenceExpressionNode(SyntaxNodeOrToken node)
        {
            var sequence = new SequenceExpressionNode();
            sequence.AddNode(node);

            return sequence;
        }

        private SequenceExpressionNode BuildSequenceExpressionNode(SyntaxNodeOrToken node, LexSpan dot, SyntaxNodeOrToken list)
        {
            var sequence = list as SequenceExpressionNode ?? new SequenceExpressionNode();
            sequence.AddNode(new DotToken(dot));
            sequence.AddNode(node);

            return sequence;
        }

        private ArgumentsNode BuildArgumentsNode()
        {
            return new ArgumentsNode();
        }

        private ArgumentsNode BuildArgumentsNode(SyntaxNodeOrToken node)
        {
            var args = new ArgumentsNode();
            args.AddNode(node);

            return args;
        }

        private ArgumentsNode BuildArgumentsNode(SyntaxNodeOrToken node, LexSpan coma, SyntaxNodeOrToken list)
        {
            var args = list as ArgumentsNode ?? new ArgumentsNode();
            args.AddNode(node);
            args.AddNode(new CommaToken(coma));

            return args;
        }

        private CallExpressionNode BuildCallExpressionNode(LexSpan func, LexSpan lPar, SyntaxNodeOrToken args, LexSpan rPar)
        {
            var callExp = new CallExpressionNode();
            callExp.AddNode(new GlobalFunctionToken(func));
            callExp.AddNode(new ParenToken(lPar));
            callExp.AddNode(args);
            callExp.AddNode(new ParenToken(rPar));

            return callExp;
        }

        private CallExpressionNode BuildCallExpressionNode(
            SyntaxNodeOrToken member, 
            LexSpan lPar, 
            SyntaxNodeOrToken args, 
            LexSpan rPar)
        {
            var callExp = new CallExpressionNode();
            callExp.AddNode(member);
            callExp.AddNode(new ParenToken(lPar));
            callExp.AddNode(args);
            callExp.AddNode(new ParenToken(rPar));

            return callExp;
        }

        private BinaryExpressionNode BuildBinaryExpressionNode(
            SyntaxNodeOrToken left, 
            SyntaxNodeOrToken opr, 
            SyntaxNodeOrToken right)
        {
            var exp = new BinaryExpressionNode();
            exp.AddNode(left);
            exp.AddNode(opr);
            exp.AddNode(right);

            return exp;
        }

        private LabelSeparatorNode BuildLabelSeparatorNode(LexSpan lex)
        {
            if(lex.token==(int)Tokens.comma)
                return new LabelSeparatorNode(new CommaToken(lex));

            if(lex.token == (int)Tokens.Eol)
                return new LabelSeparatorNode(new EolToken(lex));

            return null;
        }

        private ArrayNode BuildArrayNode()
        {
            return new ArrayNode();
        }

        private ArrayNode BuildArrayNode(SyntaxNodeOrToken node, SyntaxNodeOrToken list)
        {
            var arr = list as ArrayNode ?? new ArrayNode();
            arr.AddNode(node);

            return arr;
        }

        private ArrayNode BuildArrayNode(SyntaxNodeOrToken sep, SyntaxNodeOrToken node, SyntaxNodeOrToken list)
        {
            var arr = list as ArrayNode ?? new ArrayNode();
            arr.AddNode(sep);
            arr.AddNode(node);

            return arr;
        }

        private ArrayNode BuildArrayNode(LexSpan lBrac, SyntaxNodeOrToken list, LexSpan rBrac)
        {
            var arr = list as ArrayNode ?? new ArrayNode();
            arr.AddNode(new BracketToken(lBrac));
            arr.AddNode(new BracketToken(rBrac));

            return arr;
        }

        private IndexExpressionNode BuildIndexExpressionNode(SyntaxNodeOrToken left, LexSpan lBrac, SyntaxNodeOrToken index, LexSpan rBrac)
        {
            var indexExp = new IndexExpressionNode();
            indexExp.AddNode(left);
            indexExp.AddNode(new BracketToken(lBrac));
            indexExp.AddNode(index);
            indexExp.AddNode(new BracketToken(rBrac));

            return indexExp;
        }

        private AssignStatementNode BuildAssignStatementNode(SyntaxNodeOrToken left, LexSpan equal, SyntaxNodeOrToken right)
        {
            var assign = new AssignStatementNode();
            assign.AddNode(left);
            assign.AddNode(new OperatorToken(equal));
            assign.AddNode(right);

            return assign;
        }

        private ReturnStatementNode BuildReturnStatementNode(LexSpan lex)
        {
            var res = new ReturnStatementNode();
            res.AddNode(new ReturnToken(lex));

            return res;
        }

        private ReturnStatementNode BuildReturnStatementNode(LexSpan lex, SyntaxNodeOrToken node)
        {
            var res = new ReturnStatementNode();
            res.AddNode(new ReturnToken(lex));
            res.AddNode(node);

            return res;
        }

        private PrintStatementMode BuildPrintStatementMode(LexSpan print, SyntaxNodeOrToken node)
        {
            var printExp = new PrintStatementMode();
            printExp.AddNode(new PrintToken(print));
            printExp.AddNode(node);

            return printExp;
        }

        private DebuggerStatementNode BuildDebuggerStatementNode(LexSpan lex)
        {
            var debug = new DebuggerStatementNode();
            debug.AddNode(new StopToken(lex));

            return debug;
        }

        private StatementNode BuildStatementNode(SyntaxNodeOrToken node)
        {
            return (StatementNode)node;
        }

        private IfStatementNode BuildIfStatementNode(
            LexSpan lIf,
            SyntaxNodeOrToken condition,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rIf)
        {
            var ifExp = new IfStatementNode();
            ifExp.AddNode(new IfToken(lIf));
            ifExp.AddNode(condition);
            ifExp.AddNode(body);
            ifExp.AddNode(new EndToken(end));
            ifExp.AddNode(new IfToken(rIf));

            return ifExp;
        }

        private IfStatementNode BuildIfStatementNode(
            LexSpan lIf,
            SyntaxNodeOrToken condition,
            SyntaxNodeOrToken body,
            LexSpan el,
            SyntaxNodeOrToken elBody,
            LexSpan end,
            LexSpan rIf)
        {
            var ifExp = new IfStatementNode();
            ifExp.AddNode(new IfToken(lIf));
            ifExp.AddNode(condition);
            ifExp.AddNode(body);
            ifExp.AddNode(new ElseToken(el));
            ifExp.AddNode(elBody);
            ifExp.AddNode(new EndToken(end));
            ifExp.AddNode(new IfToken(rIf));

            return ifExp;
        }

        private StatementListNode BuildStatementListNode()
        {
            var list = new StatementListNode();

            return list;
        }

        private StatementListNode BuildStatementListNode(SyntaxNodeOrToken statement, SyntaxNodeOrToken list)
        {
            var stList = list as StatementListNode ?? new StatementListNode();
            stList.AddNode(statement);

            return stList;
        }

        private ForStatementNode BuildForStatementNode(
            LexSpan lFor,
            SyntaxNodeOrToken assign,
            LexSpan to,
            SyntaxNodeOrToken limit,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rFor)
        {
            var exp = new ForStatementNode();
            exp.AddNode(new ForToken(lFor));
            exp.AddNode(assign);
            exp.AddNode(new ToToken(to));
            exp.AddNode(limit);
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new ForToken(rFor));

            return exp;
        }

        private ForEachStatementNode BuildForEachStatementNode(
            LexSpan lFor,
            LexSpan each,
            LexSpan ident,
            LexSpan inSpan, 
            SyntaxNodeOrToken list,
            SyntaxNodeOrToken body,
            LexSpan end, 
            LexSpan rFor)
        {
            var exp = new ForEachStatementNode();
            exp.AddNode(new ForToken(lFor));
            exp.AddNode(new EachToken(each));
            exp.AddNode(new IdentToken(ident));
            exp.AddNode(new InToken(inSpan));
            exp.AddNode(list);
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new ForToken(rFor));

            return exp;
        }

        private WhileStatementNode BuildWhileStatementNode(
            LexSpan lWhile,
            SyntaxNodeOrToken condition,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rWhile)
        {
            var exp = new WhileStatementNode();
            exp.AddNode(new WhileToken(lWhile));
            exp.AddNode(condition);
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new WhileToken(rWhile));

            return exp;
        }

        private ConditionExpressionNode BuildConditionExpressionNode(SyntaxNodeOrToken node)
        {
            var exp = new ConditionExpressionNode();
            exp.AddNode(node);

            return exp;
        }

        private ConditionExpressionNode BuildConditionExpressionNode(SyntaxNodeOrToken left, LexSpan op, SyntaxNodeOrToken right)
        {
            var exp = new ConditionExpressionNode();
            exp.AddNode(left);
            if(op.token == (int)Tokens.bsAnd)
                exp.AddNode(new AndToken(op));
            else if (op.token == (int)Tokens.bsOr)
                exp.AddNode(new OrToken(op));
            else
                throw new NotImplementedException();
            exp.AddNode(right);

            return exp;
        }

        private FunctionExpressionNode BuildFunctionExpressionNode(
            LexSpan lFunc,
            LexSpan lPar,
            SyntaxNodeOrToken paramters,
            LexSpan rPar,
            SyntaxNodeOrToken type,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rFunc)
        {
            var exp = new FunctionExpressionNode();
            exp.AddNode(new FunctionToken(lFunc));
            exp.AddNode(new ParenToken(lPar));
            exp.AddNode(paramters);
            exp.AddNode(new ParenToken(rPar));
            exp.AddNode(type);
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new FunctionToken(rFunc));

            return exp;
        }

        private LabelledStatementNode BuildLabelledStatementNode(
            LexSpan ident,
            LexSpan colon,
            SyntaxNodeOrToken node
            )
        {
            var exp = new LabelledStatementNode();
            exp.AddNode(new IdentToken(ident));
            exp.AddNode(new ColonToken(colon));
            exp.AddNode(node);

            return exp;
        }

        private BlockNode BuildBlockNode()
        {
            var exp = new BlockNode();

            return exp;
        }

        private BlockNode BuildBlockNode(
            SyntaxNodeOrToken statment,
            SyntaxNodeOrToken list)
        {
            var exp = list as BlockNode ?? new BlockNode();
            exp.AddNode(statment);

            return exp;
        }

        private BlockNode BuildBlockNode(
            SyntaxNodeOrToken sep,
            SyntaxNodeOrToken statment,
            SyntaxNodeOrToken list)
        {
            var exp = list as BlockNode ?? new BlockNode();
            exp.AddNode(sep);
            exp.AddNode(statment);

            return exp;
        }

        private BlockNode BuildBlockNode(
            LexSpan lBrace,
            SyntaxNodeOrToken list,
            LexSpan rBrace)
        {
            var exp = list as BlockNode ?? new BlockNode();
            exp.AddNode(new BracketToken(lBrace));
            exp.AddNode(new BracketToken(rBrace));

            return exp;
        }

        private SubDeclarationNode BuildSubDeclarationNode(
            LexSpan lSub,
            LexSpan ident,
            LexSpan lPar,
            SyntaxNodeOrToken parameters,
            LexSpan rPar,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rSub)
        {
            var exp = new SubDeclarationNode();
            exp.AddNode(new SubToken(lSub));
            exp.AddNode(new IdentToken(ident));
            exp.AddNode(new ParenToken(lPar));
            exp.AddNode(parameters);
            exp.AddNode(new ParenToken(rPar));
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new SubToken(rSub));

            return exp;
        }

        private FunctionDeclarationNode BuildFunctionDeclarationNode(
            LexSpan lFunc,
            LexSpan ident,
            LexSpan lPar,
            SyntaxNodeOrToken parameters,
            LexSpan rPar,
            SyntaxNodeOrToken type,
            SyntaxNodeOrToken body,
            LexSpan end,
            LexSpan rFunc)
        {
            var exp = new FunctionDeclarationNode();
            exp.AddNode(new FunctionToken(lFunc));
            exp.AddNode(new IdentToken(ident));
            exp.AddNode(new ParenToken(lPar));
            exp.AddNode(parameters);
            exp.AddNode(new ParenToken(rPar));
            exp.AddNode(type);
            exp.AddNode(body);
            exp.AddNode(new EndToken(end));
            exp.AddNode(new FunctionToken(rFunc));

            return exp;
        }

        private SourceElementNode BuildSourceElementNode(SyntaxNodeOrToken node)
        {
            return (SourceElementNode) node;
        }

        private SourceElementListNode BuildSourceElementsNode()
        {
            return new SourceElementListNode();
        }

        private SourceElementListNode BuildSourceElementsNode(
            SyntaxNodeOrToken node,
            SyntaxNodeOrToken list)
        {
            var exp = list as SourceElementListNode ?? new SourceElementListNode();
            exp.AddNode(node);

            return exp;
        }

        private RootNode BuildProgramNode(SyntaxNodeOrToken node)
        {
            var exp = new RootNode();
            exp.AddNode(node);
            return exp;
        }

        public RootNode GetASTRoot()
        {
            return CurrentSemanticValue as RootNode;
        }
    }
}