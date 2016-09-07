
// =============================================================

%using System.Collections;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%visibility public
%scannertype ScannerColor
%tokentype TokensColor

%option babel, caseinsensitive, stack, classes, minimize, parser, summary, unicode, verbose, persistbuffer, noembedbuffers, out:..\..\SpecFiles\ScannerColor.cs

// =============================================================
// =============================================================

Eol             (\r\n?|\n)
Space           [ \t]
Ident           [a-zA-Z_][a-zA-Z0-9_]*
Num				[0-9]+
Real			([0-9]+"."[0-9]*)|([0-9]*"."[0-9]+)

// BrightScript Type.txt
Typs			(string|inte(rface|ger)|object|double|float|b(oolean|rsub)|void)

// BrightScript Statements.txt
Keywords		(s(t(op|ep)|ub)|i(n|f)|t(hen|o)|dim|print|e(nd(if|(sub|f(or|unction)|while))?|lse(if)?|xit(for|while)?|ach)|f(or|unction)|as|while|re(turn|m)|goto)

// BrightScript Functions.txt
BuiltInFuncs	(GetLastRun(RuntimeError|CompileError)|R(nd|un)|Box|Type)
GlobalFuncs		(Re(adAsciiFile|bootSystem)|GetInterface|MatchFiles|Sleep|C(opyFile|reate(Directory|Object))|Delete(Directory|File)|UpTime|FormatDrive|ListDir|W(ait|riteAsciiFile))
StringFuncs		(Right|Mid|Str(i(ng(i)?)?)?|Chr|Instr|UCase|Val|Asc|L(Case|e(n|ft)))
MathFuns		(S(in|qr|gn)|C(sng|dbl|os)|Tan|Int|Exp|Fix|Log|A(tn|bs))

DotChr			[^\r\n]

Opr				\(|\)|\[|\]|\{|\}|=|>=|<zz|>|<|<>|\+|-|\*|\/|\^|&|\b(?i:(And|Not|Or))\\b

Number			{Num}|{Real}
Cmnt			\'{DotChr}*
Str				\"{DotChr}*\"
Funcs			{BuiltInFuncs}|{GlobalFuncs}|{StringFuncs}|{MathFuns}
Literal			true|false|Invalid


// =============================================================
%%  // Start of rules
// =============================================================

LINE_NUM 		|
NEXT  			|
OBJFUN			|
POS 			|
RND 			|
TAB				{ return (int)TokensColor.reserved; }

{Literal}		{ return (int)TokensColor.literal; }
{Cmnt}			{ return (int)TokensColor.cmnt; }
{Keywords}		{ return (int)TokensColor.keyword; }
{Typs}			{ return (int)TokensColor.type; }
{Str}			{ return (int)TokensColor.str; }
{Funcs}			{ return (int)TokensColor.funcs; }
{Number}		{ return (int)TokensColor.number; }
{Ident}			{ return (int)TokensColor.ident; }
{Opr}			{ return (int)TokensColor.opr; }