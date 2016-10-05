﻿namespace BrightScriptTools.Compiler.AST
{
    public enum SyntaxKind
    {
        //Token Types
        EndKeyword,
        Identifier,
        OpenBracket,
        CloseBracket,
        OpenParen,
        CloseParen,
        OpenCurlyBrace,
        CloseCurlyBrace,
        Number,
        String,
        ThenKeyword,
        ElseKeyword,
        ElseIfKeyword,
        MinusOperator,
        EqualityOperator,
        PlusOperator,
        MultiplyOperator,
        DivideOperator,
        FloorDivideOperator,
        BitwiseAndOperator,
        BitwiseRightOperator,
        BitwiseOrOperator,
        Dot,
        Comma,
        Semicolon,
        Colon,
        DoubleColon,
        AssignmentOperator,
        LessThanOperator,
        GreaterThanOperator,
        StringConcatOperator,
        BitwiseLeftOperator,
        AndBinop,
        BreakKeyword,
        FalseKeyValue,
        ForKeyword,
        FunctionKeyword,
        IfKeyword,
        InKeyword,
        LocalKeyword,
        InvalidKeyValue,
        NotUnop,
        OrBinop,
        WhileKeyword,
        TrueKeyValue,
        ReturnKeyword,
        MissingToken,
        UnterminatedString,
        AsKeyword,
        Type,
        OperatorKeyword,
        GlobalFunctionToken,
        EolToken,
        PrintKeyword,
        StopKeyword,
        ToKeyword,
        EachKeyword,

        //Node Types
        ChunkNode,
        BlockNode,
        SemiColonStatementNode,
        FunctionCallStatementNode,
        ReturnStatementNode,
        BreakStatementNode,
        WhileStatementNode,
        GlobalFunctionStatementNode,
        LocalAssignmentStatementNode,
        LocalFunctionStatementNode,
        ForStatementNode,
        ForEachStatementNode,
        AssignmentStatementNode,
        IfStatementNode,
        ElseBlockNode,
        ElseIfBlockNode,
        SimpleExpression,
        BinaryOperatorExpression,
        UnaryOperatorExpression,
        FunctionDef,
        BracketField,
        AssignmentField,
        ExpField,
        SquareBracketVar,
        FunctionCallExp,
        FuncBodyNode,
        FuncNameNode,
        TypeNode,
        ParameterNode,
        ParameterListNode,
        LiteralNode,
        MemberExpressionNode,
        SequenceExpressionNode,
        ArgumentsNode,
        CallExpressionNode,
        LabelSeparatorNode,
        ArrayNode,
        IndexExpressionNode,
        PrintStatementMode,
        DebuggerStatementNode,
        StatementListNode,
        ConditionExpressionNode,

        //Trivia type
        Whitespace,
        Comment,
        Newline
    }
}