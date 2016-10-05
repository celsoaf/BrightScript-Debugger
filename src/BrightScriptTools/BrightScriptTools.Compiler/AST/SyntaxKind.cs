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
        InvalidKeyValue,
        NotUnop,
        OrBinop,
        WhileKeyword,
        TrueKeyValue,
        ReturnKeyword,
        MissingToken,
        AsKeyword,
        Type,
        OperatorKeyword,
        GlobalFunctionToken,
        EolToken,
        PrintKeyword,
        StopKeyword,
        ToKeyword,
        EachKeyword,
        SubKeyword,

        //Node Types
        ChunkNode,
        BlockNode,
        ReturnStatementNode,
        WhileStatementNode,
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
        FunctionStatementDef,
        SubStatementDef,
        FunctionCallExp,
        TypeNode,
        ParameterNode,
        ParameterListNode,
        LiteralNode,
        MemberExpressionNode,
        SequenceExpressionNode,
        ArgumentsNode,
        LabelSeparatorNode,
        ArrayNode,
        IndexExpressionNode,
        PrintStatementMode,
        DebuggerStatementNode,
        StatementListNode,
        ConditionExpressionNode,
        LabelledStatementNode,
        SourceElementListNode,

        //Trivia type
        Whitespace,
        Comment,
        Newline
    }
}