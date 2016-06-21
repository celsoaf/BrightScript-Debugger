
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan

%token	bar, dot, semi, star, lt, gt, ltEqual, gtEqual, notEqual, comma, slash, lBrac, rBrac, lPar, rPar, lBrace, rBrace, Eol, equal, plus, minus, questionMark, colon

%token bsIdent, bsNumber, bsStr, bsCmnt, bsFuncs, bsType, bsAs, bsTrue, bsFalse, bsInvalid, bsNot, bsM, bsStop, bsReturn, bsPrint

%token bsIf, bsElse, bsFor, bsTo, bsEach, bsStep, bsIn, bsWhile

%token bsSub, bsFunction, bsEnd

%token maxParseToken EOL comment errTok repErr

%%

Program
    : SourceElements EOF
    ;

SourceElements
	: EolOpt /* Empty */
	| EolOpt SourceElement SourceElements
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
	: /* Empty */
	| Parameter ParameterTail
	;

ParameterTail
	: /* Empty */
	| comma Parameter ParameterTail
	;

Parameter
	: bsIdent Type 
	| bsIdent equal Literal Type
	;

Type
	: /* Empty */
	| bsAs bsType
	;

EolOpt
	: /* Empty */
	| Eol EolOpt
	;

StatementList
	: EolOpt /* Empty */
	| EolOpt Statement StatementList 
	;

Statement
	: DebuggerStatement
	| PrintStatement
	| SequenceExpression
	| IterationStatement
	| IfStatement
	| AssignStatement
	| ReturnStatement
	;

AssignStatement
	: SequenceExpression equal SequenceExpression
	;

IfStatement
	: bsIf SequenceExpression StatementList bsEnd bsIf
	| bsIf SequenceExpression StatementList bsElse StatementList bsEnd bsIf
	;

IterationStatement
	: bsFor AssignStatement bsTo SequenceExpression StatementList bsEnd bsFor
	| bsFor bsEach bsIdent bsIn SequenceExpression StatementList bsEnd bsFor
	| bsWhile SequenceExpression StatementList bsEnd bsWhile
	;

ReturnStatement
	: bsReturn SequenceExpression
	| bsReturn Eol
	;

DebuggerStatement
	: bsStop
	;

PrintStatement
	: bsPrint SequenceExpression
	| questionMark SequenceExpression
	;
	
SequenceExpression
	: SingleExpression
	| SingleExpression dot SequenceExpression
	;

SingleExpression
	: Block 
	| MemberExpression
	| BinaryExpression
	| UnaryExpression
	| CallExpression
	| Literal
	;

Block
	: lBrace LabelledStatementList rBrace
	;

LabelledStatementList
	: EolOpt /* Empty */
	| EolOpt LabelledStatement LabelledStatementTail
	;

LabelledStatementTail
	: EolOpt /* Empty */
	| LabelSeparator LabelledStatement LabelledStatementTail
	;

LabelledStatement
	: bsIdent colon FunctionStatement
	| bsIdent colon SequenceExpression
	;

FunctionStatement
	: bsFunction lPar ParameterList rPar Type StatementList bsEnd bsFunction
	;

LabelSeparator
	: Eol EolOpt
	| comma
	;

CallExpression
	: bsFuncs lPar Arguments rPar
	| MemberExpression lPar Arguments rPar
	;

Arguments
	: /* Empty */
	| SequenceExpression 
	| SequenceExpression comma Arguments
	;

UnaryExpression
	: lPar SequenceExpression rPar 
	| minus SingleExpression
	| bsNot SingleExpression
	;

BinaryExpression
	: MemberExpression MathOperator SingleExpression
	| MemberExpression BooleanOperator SingleExpression
	;

MemberExpression
	: bsIdent
	| bsIdent lBrac SingleExpression rBrac
	;

MathOperator
	: plus
	| minus
	| star
	| slash
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
	: EmptyBlock
	| NullLiteral
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

EmptyBlock
	:  lBrace rBrace
	;
%%