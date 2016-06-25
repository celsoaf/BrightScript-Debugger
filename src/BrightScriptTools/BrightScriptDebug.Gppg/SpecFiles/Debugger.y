
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
%token dgTraceFile, dgVariables, dgDebugger, dgNote

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
	| BacktraceStatment
	| VariablesStatment
	| DebuggerStatment
	| AppCloseStatement
	;

CompilingStatment
	: dgMinus dgCompiling dgIdent dgStr dgMinus Eol
	;

RunningStatment
	: dgMinus dgRunning dgIdent dgStr dgIdent dgMinus Eol { ProcessAppOpen(); }
	;

EnterDebugStatment
	: dgEnterDebug Eol
	;

CurrentFunctionStatment
	: dgCurrFunc Eol { ProcessCurrentFunction(); }
	;

BacktraceStatment
	: dgBacktrace Eol { ProcessBacktrace(); }
	;

VariablesStatment
	: dgVariables Eol { ProcessVariables(); }
	;

DebuggerStatment
	: dgDebugger Eol { ProcessDebug(); }
	| dgDebugger dgNote Eol  { ProcessAppClose(); }
	;

AppCloseStatement
	: dgNote Eol { ProcessAppClose(); }
	; 

ErrorStatment
	: errTok Eol
	| error Eol
	;


%%