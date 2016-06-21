
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptDebug.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan

%token dot, colon, star, gt, comma, slash, lPar, rPar, lBrace, rBrace, Eol, equal, minus

%token print

%token dgStop, dgFunction, dgBS, dgDebugger, dgLocal, dgVariables, dgFile, dgLine, dgBacktrace, dgCurrent, dgCompiling, dgRunning, dgDev, dgMain, dgMinus, dgDebug

%token dgNumber, dgIdent, dgStr

%token maxParseToken EOL comment errTok repErr

%%

Program
    : DebugElements EOF
    ;

DebugElements
	: EolOpt DebugElement DebugElements 
	| EolOpt /* Empty */
	;

DebugElement
	: CompileStatment
	| RunStatment
	| CurrentFunctionStatement
	| BacktraceStatment
	| LocalVariablesStatment
	| InputStatment
	| ErrorStatment
	;

CompileStatment
	: dgMinus dgCompiling dgDev dgStr dgMinus
	;

RunStatment
	: dgMinus dgRunning dgDev dgStr dgMain dgMinus
	;

CurrentFunctionStatement
	: dgCurrent dgFunction colon
	;

BacktraceStatment
	: dgBacktrace colon
	;

LocalVariablesStatment
	: dgLocal dgVariables colon
	;

InputStatment
	: dgBS dgDebugger gt
	;

ErrorStatment
	: dgStr
	;


EolOpt
	: Eol EolOpt
	| /* Empty */
	;

%%