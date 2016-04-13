
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

%token bsIdent, bsNumber, bsStr, bsCmnt, bsFuncs, bsType, bsAs, bsTrue, bsFalse, bsInvalid, bsNot, bsM, bsStop, bsReturn

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
	| SingleExpression
	;

AssignStatement
	: MemberExpression equal SingleExpression
	;

IfStatement
	: bsIf BooleanExpression StatementList bsEnd bsIf
	| bsIf BooleanExpression StatementList bsElse StatementList bsEnd bsIf
	;

IterationStatement
	: bsFor SingleExpression bsTo SingleExpression StatementList bsEnd bsFor
	| bsFor bsEach SingleExpression bsIn SingleExpression StatementList bsEnd bsFor
	| bsWhile BooleanExpression StatementList bsEnd bsWhile
	;

ReturnStatement
	: bsReturn SingleExpression
	| bsReturn Eol
	;

DebuggerStatement
	: bsStop
	;
	
SingleExpression
	: UnaryExpression 
	| CallExpression
	//| BinaryExpression
	| Literal
	;

BooleanExpression
	: bsTrue
	| bsFalse
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
	: bsNot SingleExpression
	| lPar SingleExpression rPar 
	| minus SingleExpression
	;

MemberExpression
	: bsIdent
	| bsIdent dot MemberExpression
	| bsIdent lBrac MemberExpression rBrac
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