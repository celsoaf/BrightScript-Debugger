
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

%token bsIdent, bsNumber, bsStr, bsCmnt, bsFuncs, bsType, bsAs, bsTrue, bsFalse, bsInvalid, bsNot, bsAnd, bsOr, bsM, bsStop, bsReturn, bsPrint

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
	: bsFunction bsIdent lPar ParameterList rPar Type StatementList bsEnd bsFunction { $$ = BuildFunctionDeclarationNode(@1, @2, @3, $4, @5, $6, $7, @8, @9); }
	;

SubDeclaration
	: bsSub bsIdent lPar ParameterList rPar StatementList bsEnd bsSub { $$ = BuildSubDeclarationNode(@1, @2, @3, $4, @5, $6, @7, @8); }
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
	: EolOpt /* Empty */					{ $$ = BuildStatementListNode(); }
	| Eol EolOpt Statement StatementList	{ $$ = BuildStatementListNode($3, $4); }
	;

Statement
	: DebuggerStatement			{ $$ = BuildStatementNode($1); }
	| PrintStatement			{ $$ = BuildStatementNode($1); }
	| SequenceExpression		{ $$ = BuildStatementNode($1); }
	| IterationStatement		{ $$ = BuildStatementNode($1); }
	| IfStatement				{ $$ = BuildStatementNode($1); }
	| AssignStatement			{ $$ = BuildStatementNode($1); }
	| ReturnStatement			{ $$ = BuildStatementNode($1); }
	;

AssignStatement
	: SequenceExpression equal SequenceExpression		{ $$ = BuildAssignStatementNode($1, @2, $3); }
	;

IfStatement
	: bsIf ConditionExpression StatementList bsEnd bsIf						{ $$ = BuildIfStatementNode(@1, $2, $3, @4, @5); }
	| bsIf ConditionExpression StatementList bsElse StatementList bsEnd bsIf	{ $$ = BuildIfStatementNode(@1, $2, $3, @4, $5, @6, @7); }
	;

IterationStatement
	: bsFor AssignStatement bsTo SequenceExpression StatementList bsEnd bsFor	{ $$ = BuildForStatementNode(@1, $2, @3, $4, $5, @6, @7); }
	| bsFor bsEach bsIdent bsIn SequenceExpression StatementList bsEnd bsFor	{ $$ = BuildForEachStatementNode(@1, @2, @3, @4, $5, $6, @7, @8); }
	| bsWhile ConditionExpression StatementList bsEnd bsWhile					{ $$ = BuildWhileStatementNode(@1, $2, $3, @4, @5); }
	;

ConditionExpression
	: SequenceExpression							{ $$ = BuildConditionExpressionNode($1); }
	| SequenceExpression bsAnd SequenceExpression	{ $$ = BuildConditionExpressionNode($1, @2, $3); }
	| SequenceExpression bsOr SequenceExpression	{ $$ = BuildConditionExpressionNode($1, @2, $3); }
	;

ReturnStatement
	: bsReturn SequenceExpression	{ $$ = BuildReturnStatementNode(@1, $2); }
	| bsReturn Eol					{ $$ = BuildReturnStatementNode(@1); }
	;

DebuggerStatement
	: bsStop							{ $$ = BuildDebuggerStatementNode(@1); }
	;

PrintStatement
	: bsPrint SequenceExpression		{ $$ = BuildPrintStatementMode(@1, $2); }
	| questionMark SequenceExpression	{ $$ = BuildPrintStatementMode(@1, $2); }
	;
	
SequenceExpression
	: SingleExpression							{ $$ = BuildSequenceExpressionNode($1); }
	| SingleExpression dot SequenceExpression	{ $$ = BuildSequenceExpressionNode($1, @2, $3); }
	;

SingleExpression
	: Block							{ $$ = BuildSingleExpressionNode($1); }
	| IndexExpression				{ $$ = BuildSingleExpressionNode($1); }
	| MemberExpression				{ $$ = BuildSingleExpressionNode($1); }
	| BinaryExpression				{ $$ = BuildSingleExpressionNode($1); }
	| UnaryExpression				{ $$ = BuildSingleExpressionNode($1); }
	| CallExpression				{ $$ = BuildSingleExpressionNode($1); }
	| Literal						{ $$ = BuildSingleExpressionNode($1); }
	;

Block
	: lBrace LabelledStatementList rBrace						{ $$ = BuildBlockNode(@1, $2, @3); }
	;

LabelledStatementList
	: EolOpt /* Empty */										{ $$ = BuildBlockNode(); }
	| EolOpt LabelledStatement LabelledStatementTail			{ $$ = BuildBlockNode($2, $3); }
	;

LabelledStatementTail
	: EolOpt /* Empty */										{ $$ = BuildBlockNode(); }
	| LabelSeparator LabelledStatement LabelledStatementTail	{ $$ = BuildBlockNode($1, $2, $3); }
	;

LabelledStatement
	: bsIdent colon FunctionExpression		{  $$ = BuildLabelledStatementNode(@1, @2, $3); }
	| bsIdent colon SequenceExpression		{  $$ = BuildLabelledStatementNode(@1, @2, $3); }
	;

FunctionExpression
	: bsFunction lPar ParameterList rPar Type StatementList bsEnd bsFunction { $$ = BuildFunctionExpressionNode(@1, @2, $3, @4, $5, $6, @7, @8); }
	;

LabelSeparator
	: Eol EolOpt		{ $$ = BuildLabelSeparatorNode(@1); }
	| comma				{ $$ = BuildLabelSeparatorNode(@1); }
	;

CallExpression
	: bsFuncs lPar Arguments rPar				{ $$ = BuildCallExpressionNode(@1, @2, $3, @4); }
	| SequenceExpression lPar Arguments rPar	{ $$ = BuildCallExpressionNode($1, @2, $3, @4); }
	;

Arguments
	: /* Empty */							{ $$ = BuildArgumentsNode(); }
	| SequenceExpression					{ $$ = BuildArgumentsNode($1); }
	| SequenceExpression comma Arguments	{ $$ = BuildArgumentsNode($1, @2, $3); }
	;

UnaryExpression
	: lPar SequenceExpression rPar	{ $$ = BuildUnaryExpressionNode(@1, $2, @3); }
	| minus SingleExpression		{ $$ = BuildUnaryExpressionNode(@1, $2); }
	| bsNot SingleExpression		{ $$ = BuildUnaryExpressionNode(@1, $2); }
	;

BinaryExpression
	: SequenceExpression MathOperator SequenceExpression		{ $$ = BuildBinaryExpressionNode($1, $2, $3); }
	| SequenceExpression BooleanOperator SequenceExpression		{ $$ = BuildBinaryExpressionNode($1, $2, $3); }
	;

MemberExpression
	: bsIdent			{ $$ = BuildMemberExpressionNode(@$); }
	;

IndexExpression
	: SequenceExpression lBrac SequenceExpression rBrac		{ $$ = BuildIndexExpressionNode($1, @2, $3, @4); }
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
	: lBrac ArrayList rBrac							{ $$ = BuildArrayNode(@1, $2, @3); }
	;

ArrayList
	: EolOpt /* Empty */							{ $$ = BuildArrayNode(); }
	| EolOpt SequenceExpression ArrayTail			{ $$ = BuildArrayNode($2, $3); }
	;

ArrayTail
	: EolOpt /* Empty */							{ $$ = BuildArrayNode(); }
	| LabelSeparator SequenceExpression ArrayTail	{ $$ = BuildArrayNode($1, $2, $3); }
	;

%%