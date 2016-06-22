
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

%token dgEnterDebug, dgCompiling, dgMinus, dgRunning, dgCurrFunc, dgCodeLine, dgDebugLine, dgBacktrace, dgTraceLine
%token dgTraceFile, dgVariables, dgDebugger, dgVarLine

%token dgNumber, dgIdent, dgStr, dgLine

%token maxParseToken EOL comment errTok repErr

%%

Program
    : DebugElements EOF
    ;

DebugElements
	: DebugElement DebugElements 
	| /* Empty */
	;

DebugElement
	: Eol
	| ErrorStatment
	| EnterDebugStatment
	| CompilingStatment
	| RunningStatment
	| CurrentFunctionStatment
	| CodeLineStatment
	| DebugLineStatment
	| BacktraceStatment
	| TraceLineStatment
	| VariablesStatment
	| DebuggerStatment
	| VarLineStatment
	;

CompilingStatment
	: dgMinus dgCompiling dgIdent dgStr dgMinus Eol
	;

RunningStatment
	: dgMinus dgRunning dgIdent dgStr dgIdent dgMinus Eol
	;

EnterDebugStatment
	: dgEnterDebug Eol
	;

CurrentFunctionStatment
	: dgCurrFunc Eol
	;

CodeLineStatment
	: dgCodeLine Eol
	;

DebugLineStatment
	: dgDebugLine Eol
	;

BacktraceStatment
	: dgBacktrace Eol
	;

TraceLineStatment
	: dgTraceLine Eol dgTraceFile Eol
	;

VariablesStatment
	: dgVariables Eol
	;

VarLineStatment
	: dgVarLine Eol
	;

DebuggerStatment
	: dgDebugger Eol
	;

ErrorStatment
	: errTok Eol
	;


%%