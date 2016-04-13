
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

%token bsIdent, bsNumber, bsStr, bsCmnt, bsFuncs, bsType, bsAs, bsTrue, bsFalse, bsInvalid, bsNot, bsM, bsStop

%token bsIf, bsElse, bsFor, bsTo, bsEach, bsStep, bsIn, bsWhile

%token bsSub, bsFunction, bsEnd

%token maxParseToken EOL comment errTok repErr

%%

Program
    : SourceElements EOF
    ;

SourceElements
	: EolOpt SourceElement SourceElements 
	| EolOpt /* Empty */
	;

SourceElement
	: SubDeclaration 
	| FunctionDeclaration
	;

FunctionDeclaration
	: bsFunction bsIdent lPar ParameterList rPar Type StatementList bsEnd bsFunction
	;

SubDeclaration
	: bsSub bsIdent lPar ParameterList rPar StatementList bsEnd bsSub 
	;

ParameterList
	: Parameter ParameterTail
	| /* Empty */
	;

ParameterTail
	: comma Parameter ParameterTail
	| /* Empty */
	;

Parameter
	: bsIdent equal Literal Type
	| bsIdent Type 
	;

Type
	: bsAs bsType
	| /* Empty */
	;

EolOpt
	: Eol EolOpt
	| /* Empty */
	;

StatementList
	: EolOpt Statement StatementList
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
	: bsIf BooleanExpression StatementList bsEnd bsIf
	| bsIf BooleanExpression StatementList bsElse StatementList bsEnd bsIf
	;

StFor
	: bsFor Expression bsTo Expression StatementList bsEnd bsFor
	| bsFor bsEach Expression bsIn Expression StatementList bsEnd bsFor
	;

StWhile
	: bsWhile BooleanExpression StatementList bsEnd bsWhile
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
*/

%%