
// =============================================================

%using System.Collections;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%visibility public

%option babel, caseinsensitive, stack, classes, minimize, parser, summary, unicode, verbose, persistbuffer, noembedbuffers, out:..\..\SpecFiles\Scanner.cs

// =============================================================
// =============================================================

Eol             (\r\n?|\n)
Space           [ \t]
Ident           [a-zA-Z_][a-zA-Z0-9_]*
Num				[0-9]+
Real			([0-9]+"."[0-9]*)|([0-9]*"."[0-9]+)

// BrightScript Type.txt
Typs			(string|in(te(rface|ger)|valid)|object|double|float|b(oolean|rsub)|void)

// BrightScript Statements.txt
Keywords		(s(t(op|ep)|ub)|i(n|f)|t(hen|o)|dim|print|e(nd(if|(sub|f(or|unction)|while))?|lse(if)?|xit(for|while)?|ach)|f(or|unction)|as|while|re(turn|m)|goto)

// BrightScript Functions.txt
BuiltInFuncs	(GetLastRun(RuntimeError|CompileError)|R(nd|un)|Box|Type)
GlobalFuncs		(Re(adAsciiFile|bootSystem)|GetInterface|MatchFiles|Sleep|C(opyFile|reate(Directory|Object))|Delete(Directory|File)|UpTime|FormatDrive|ListDir|W(ait|riteAsciiFile))
StringFuncs		(Right|Mid|Str(i(ng(i)?)?)?|Chr|Instr|UCase|Val|Asc|L(Case|e(n|ft)))
MathFuns		(S(in|qr|gn)|C(sng|dbl|os)|Tan|Int|Exp|Fix|Log|A(tn|bs))

DotChr			[^\r\n]

Number			{Num}|{Real}
Cmnt			\'{DotChr}*
Str				\"{DotChr}*\"
Funcs			{BuiltInFuncs}|{GlobalFuncs}|{StringFuncs}|{MathFuns}


// =============================================================
%%  // Start of rules
// =============================================================

AND  			|
AS				|
DIM  			|
CREATEOBJECT	| 
EACH			|
ELSE  			|
END 			|
ENDFOR 			|
ENDFUNCTION		|
ENDIF			|
ENDSUB			|
ENDWHILE 		|
EXIT			|
EXITWHILE 		|
FALSE 			|
FOR  			|
FUNCTION 		|
GOTO  			|
IF  			|
INTEGER			|
INVALID  		|
LINE_NUM 		|
M				|
NEXT  			|
NOT  			|
OR  			|
OBJECT			|
OBJFUN			|
POS 			|
PRINT  			|
RND 			|
REM 			|
RETURN 			|
STEP 			|
STOP 			|
STRING			|
SUB 			|
TAB				|
THEN  			|
TO 				|
TRUE			|
TYPE 			|
VOID			|
WHILE			{ return (int)Tokens.keyword; }
{Cmnt}			{ return (int)Tokens.cmnt; }
{Keywords}		{ return (int)Tokens.keyword; }
{Typs}			{ return (int)Tokens.type; }
{Str}			{ return (int)Tokens.str; }
{Funcs}			{ return (int)Tokens.funcs; }
{Ident}			{ return (int)Tokens.ident; }
{Number}		{ return (int)Tokens.number; }