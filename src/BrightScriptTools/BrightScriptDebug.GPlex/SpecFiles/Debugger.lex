
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


dgEnterDebug	BrightScript\x20Micro\x20Debugger\.|Enter\x20any\x20BrightScript\x20statement\,\x20debug\x20commands\,\x20or\x20HELP\.
dgMinus			------
dgCompiling		Compiling
dgRunning		Running
dgCurrFunc		Current\x20Function:
dgCodeLine		{Num}{colon}{DotChr}*
dgDebugLine		STOP{DotChr}*
dgBacktrace		Backtrace:
dgTraceLine		#{Num}{DotChr}*
dgTraceFile		file\/line{DotChr}*
dgVariables		Local\x20Variables:
dgVarLine		{Ident}\t{DotChr}*
dgDebugger		BrightScript\x20Debugger>

Number			{Num}|{Real}
Str				'{DotChr}*'
Line			{DotChr}*


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

{dgEnterDebug}	{ return (int)Tokens.dgEnterDebug; }
{dgMinus}		{ return (int)Tokens.dgMinus; }
{dgCompiling}	{ return (int)Tokens.dgCompiling; }
{dgRunning}		{ return (int)Tokens.dgRunning; }
{dgCurrFunc}	{ return (int)Tokens.dgCurrFunc; }
{dgCodeLine}	{ return (int)Tokens.dgCodeLine; }
{dgDebugLine}	{ return (int)Tokens.dgDebugLine; }
{dgBacktrace}	{ return (int)Tokens.dgBacktrace; }
{dgTraceLine}	{ return (int)Tokens.dgTraceLine; }
{dgTraceFile}	{ return (int)Tokens.dgTraceFile; }
{dgVariables}	{ return (int)Tokens.dgVariables; }
{dgVarLine}		{ return (int)Tokens.dgVarLine; }
{dgDebugger}	{ return (int)Tokens.dgDebugger; }

{Number}		{ return (int)Tokens.dgNumber; }
{Ident}			{ return (int)Tokens.dgIdent; }
{Str}			{ return (int)Tokens.dgStr; }
//{Line}			{ return (int)Tokens.dgLine; }