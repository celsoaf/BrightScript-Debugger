
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;
%using BrightScriptTools.Compiler.AST
%using BrightScriptTools.Compiler.AST.Tokens

%namespace BrightScriptTools.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan
%YYSTYPE SyntaxNodeOrToken

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
	: /* Empty */					{ $$ = BuildParameterListNode(); }
	| Parameter ParameterTail		{ $$ = BuildParameterListNode($1, $2); }
	;

ParameterTail
	: /* Empty */					{ $$ = BuildParameterListNode(); }
	| comma Parameter ParameterTail	{ $$ = BuildParameterListNode(@1, $2, $3); }
	;

Parameter
	: bsIdent Type					{ $$ = BuildParameterNode(@1, $2); }
	| bsIdent equal Literal Type	{ $$ = BuildParameterNode(@1, @2, $3, $4); }
	;

Type
	: /* Empty */
	| bsAs bsType					{ $$ = BuildTypeNode(@1, @2); }
	;

EolOpt
	: /* Empty */
	| Eol EolOpt
	;

StatementList
	: EolOpt /* Empty */
	| Eol EolOpt Statement StatementList 
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
	| IndexExpression
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
	| SequenceExpression lPar Arguments rPar
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
	: SequenceExpression MathOperator SequenceExpression
	| SequenceExpression BooleanOperator SequenceExpression
	;

MemberExpression
	: bsIdent 
	;

IndexExpression
	: SequenceExpression lBrac SequenceExpression rBrac
	;

MathOperator
	: plus				{ $$ = BuildMathOperatorNode(@$); }
	| minus				{ $$ = BuildMathOperatorNode(@$); }
	| star				{ $$ = BuildMathOperatorNode(@$); }
	| slash				{ $$ = BuildMathOperatorNode(@$); }
	;

BooleanOperator
	: lt				{ $$ = BuildBooleanOperatorNode(@$); }
	| ltEqual			{ $$ = BuildBooleanOperatorNode(@$); }
	| gt				{ $$ = BuildBooleanOperatorNode(@$); }
	| gtEqual			{ $$ = BuildBooleanOperatorNode(@$); }
	| equal				{ $$ = BuildBooleanOperatorNode(@$); }
	| notEqual			{ $$ = BuildBooleanOperatorNode(@$); }
	;
	
Literal
	: EmptyBlock		{ $$ = BuildLiteralNode($1); }
	| Array				{ $$ = BuildLiteralNode($1); }
	| NullLiteral		{ $$ = BuildLiteralNode($1); }
	| BooleanLiteral	{ $$ = BuildLiteralNode($1); }
	| StringLiteral		{ $$ = BuildLiteralNode($1); }
	| NumericLiteral	{ $$ = BuildLiteralNode($1); }
	;

NullLiteral
	: bsInvalid			{ $$ = BuildInvalidNode(@$); }
	;

BooleanLiteral 
	: bsTrue			{ $$ = BuildBooleanNode(@$, true); }
	| bsFalse			{ $$ = BuildBooleanNode(@$, false); }
	;

StringLiteral
	: bsStr				{ $$ = BuildStringNode(@$); }
	;

NumericLiteral
	: bsNumber			{ $$ = BuildNumberNode(@$); }
	;

EmptyBlock
	:  lBrace rBrace	{ $$ = BuildEmptyBlock(@1, @2); }
	;

Array
	: lBrac ArrayList rBrac
	;

ArrayList
	: EolOpt /* Empty */
	| EolOpt SequenceExpression ArrayTail
	;

ArrayTail
	: EolOpt /* Empty */
	| LabelSeparator SequenceExpression ArrayTail
	;

%%