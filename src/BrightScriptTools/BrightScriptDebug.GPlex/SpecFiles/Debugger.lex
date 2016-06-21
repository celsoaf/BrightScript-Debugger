
// =============================================================

%using System.Collections;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptDebug.Compiler

%visibility public

%option babel, caseinsensitive, stack, classes, minimize, parser, summary, unicode, verbose, persistbuffer, noembedbuffers, out:..\..\SpecFiles\Scanner.cs

// =============================================================
// =============================================================

Eol             (\r\n?|\n)
Space           [ \t]
Ident           [a-zA-Z_][a-zA-Z0-9_]*
Num				[0-9]+
Real			([0-9]+"."[0-9]*)|([0-9]*"."[0-9]+)

DotChr			[^\r\n]

dot				\. 
colon			:
star			\* 
gt				> 
comma			\, 
slash			\/ 
lPar			\(
rPar			\) 
lBrace			\{	
rBrace			\}
equal			=
minus			-
questionMark	\?
quote			'

print			print 

dgStop			stop
dgFunction		Function
dgBS			BrightScript
dgDebugger		Debugger
dgLocal			Local
dgVariables		Variables
dgFile			File
dgLine			Line
dgBacktrace		Backtrace
dgCurrent		Current
dgCompiling		Compiling
dgRunning		Running
dgDev			dev
dgMain			main
dgMinus			------
dgMicro			Micro
dgEnter			Enter
dgAny			any
dgStatement		statement
dgDebug			debug
dgCommands		commands
dgOr			or
dgHelp			help

Number			{Num}|{Real}
Str				'{DotChr}*'
ColonStr		:{DotChr}*


// =============================================================
%%  // Start of rules
// =============================================================


{dot}			{ return (int)Tokens.dot; }
{colon}			{ return (int)Tokens.colon; }
{star}			{ return (int)Tokens.star; }
{gt}			{ return (int)Tokens.gt; }
{comma}			{ return (int)Tokens.comma; }
{slash}			{ return (int)Tokens.slash; }
{lPar}			{ return (int)Tokens.lPar; }
{rPar}			{ return (int)Tokens.rPar; }
{lBrace}		{ return (int)Tokens.lBrace; }
{rBrace}		{ return (int)Tokens.rBrace; }
{Eol}			{ return (int)Tokens.Eol;}
{equal}			{ return (int)Tokens.equal;}
{minus}			{ return (int)Tokens.minus;}

{print}			{ return (int)Tokens.print; }

{dgStop}		{ return (int)Tokens.dgStop; }
{dgFunction}	{ return (int)Tokens.dgFunction; }
{dgBS}			{ return (int)Tokens.dgBS; }
{dgDebugger}	{ return (int)Tokens.dgDebugger; }
{dgLocal}		{ return (int)Tokens.dgLocal; }
{dgVariables}	{ return (int)Tokens.dgVariables; }
{dgFile}		{ return (int)Tokens.dgFile; }
{dgLine}		{ return (int)Tokens.dgLine; }
{dgBacktrace}	{ return (int)Tokens.dgBacktrace; }
{dgCurrent}		{ return (int)Tokens.dgCurrent; }
{dgCompiling}	{ return (int)Tokens.dgCompiling; }
{dgRunning}		{ return (int)Tokens.dgRunning; }
{dgDev}			{ return (int)Tokens.dgDev; }
{dgMain}		{ return (int)Tokens.dgMain; }
{dgMinus}		{ return (int)Tokens.dgMinus; }

{Number}		{ return (int)Tokens.dgNumber; }
{Ident}			{ return (int)Tokens.dgIdent; }
{Str}			{ return (int)Tokens.dgStr; }
