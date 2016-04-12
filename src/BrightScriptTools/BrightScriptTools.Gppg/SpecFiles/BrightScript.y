﻿
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan

%token	bar, dot, semi, star, lt, gt, ltEqual, gtEqual, notEqual, comma, slash, lBrac, rBrac, lPar, rPar, lBrace, rBrace, Eol, equal, plus, minus

%token bsIdent, bsNumber, bsStr, bsCmnt, bsFuncs, bsType, bsAs, bsTrue, bsFalse, bsInvalid, bsNot

%token bsIf, bsElse, bsFor, bsTo, bsEach, bsStep, bsIn, bsWhile

%token bsSub, bsFunction, bsEnd

%token maxParseToken EOL comment errTok repErr

%%

Program
    : FunctionSeq EOF
    ;

FunctionSeq
	: EolOpt FunctionElem FunctionSeq 
	| EolOpt /* Empty */
	;

FunctionElem
	: Sub 
	| Function
	;

Function
	: bsFunction bsIdent lPar ParamSeq rPar AsBlock StatementSeq bsEnd bsFunction
	;

Sub
	: bsSub bsIdent lPar ParamSeq rPar StatementSeq bsEnd bsSub 
	;

ParamSeq
	: Param ParamTail
	| /* Empty */
	;

ParamTail
	: comma Param ParamTail
	| /* Empty */
	;

Param
	: bsIdent AsBlock
	;

AsBlock
	: bsAs bsType
	| /* Empty */
	;

EolOpt
	: Eol EolOpt
	| /* Empty */
	;

StatementSeq
	: EolOpt Statement StatementSeq
	| EolOpt /* Empty */
	;

Statement
	: StAssign
	| StIf
	| StFor
	| StWhile
	| Expression
	;

StAssign
	: bsIdent equal Expression
	| bsIdent lBrac Expression rBrac equal Expression
	;

StIf
	: bsIf BooleanExpression StatementSeq bsEnd bsIf
	| bsIf BooleanExpression StatementSeq bsElse StatementSeq bsEnd bsIf
	;

StFor
	: bsFor Expression bsTo Expression StatementSeq bsEnd bsFor
	| bsFor bsEach Expression bsIn Expression StatementSeq bsEnd bsFor
	;

StWhile
	: bsWhile BooleanExpression StatementSeq bsEnd bsWhile
	;
	
Expression
	: UnaryExpression 
	//| BinaryExpression
	| Operand
	;

BooleanExpression
	: bsTrue
	| bsFalse
	| Expression BooleanOperator Expression
	;

//CallExpression
//	:
//	;

UnaryExpression
	: bsNot Expression
	| lPar Expression rPar 
	| minus Expression
	;

Operand
	: bsIdent
	| bsNumber
	| bsStr
	| bsTrue
	| bsFalse
	| bsInvalid
	;

BooleanOperator
	: lt
	| ltEqual
	| gt
	| gtEqual
	| equal
	| notEqual
	;

/*
BinaryExpression
	: LeftOperand Operator RightOperand
	;

LeftOperand
	: Operand
	| Expression
	;

RightOperand
	: Operand
	| Expression
	;

Operator
	: lt
	| gt
	| slash
	| star
	| minus
	| plus 
	;

Literal
	: NullLiteral
	| BooleanLiteral
	| StringLiteral
	| NumericLiteral
	;

NullLiteral
	: bsInvalid
	;

BooleanLiteral
	: bsTrue
	| bsFalse
	;

StringLiteral
	: bsStr
	;

NumericLiteral
	: bsNumber
	;
*/

%%