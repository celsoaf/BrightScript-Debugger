
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
	: AssignStatement
	| IfStatement
	| IterationStatement
	| ReturnStatement
	| DebuggerStatement
	| PrintStatement
	| SequenceExpression
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
	| UnaryExpression 
	| CallExpression
	| BinaryExpression
	| Literal
	;

Block
	: lBrace LabelledStatementList rBrace
	;

LabelledStatementList
	: EolOpt LabelledStatement LabelledStatementTail
	| EolOpt /* Empty */
	;

LabelledStatementTail
	: LabelSeparator LabelledStatement LabelledStatementTail
	| EolOpt /* Empty */
	;

LabelledStatement
	: bsIdent colon SequenceExpression
	| bsIdent colon FunctionStatement
	;

FunctionStatement
	: bsFunction lPar ParameterList rPar Type StatementList bsEnd bsFunction
	;

LabelSeparator
	: Eol EolOpt
	| comma
	;

BooleanExpression
	: BooleanLiteral
	| bsNot BooleanExpression
	| SingleExpression BooleanOperator SingleExpression
	;

CallExpression
	: MemberExpression lPar Arguments rPar
	| bsFuncs lPar Arguments rPar
	;

Arguments
	: SingleExpression Arguments
	| /* Empty */
	;

UnaryExpression
	: lPar SingleExpression rPar 
	| minus SingleExpression
	;

BinaryExpression
	: MemberExpression plus SingleExpression
	| MemberExpression minus SingleExpression
	| MemberExpression star SingleExpression
	| MemberExpression slash SingleExpression
	;

MemberExpression
	: bsIdent
	| bsIdent lBrac SingleExpression rBrac
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

%%