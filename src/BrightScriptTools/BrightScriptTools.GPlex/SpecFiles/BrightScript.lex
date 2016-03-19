
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

DotChr			[^\r\n]

OneLineCmnt		\'{DotChr}*
Number			{Num}|{Real}


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
{Ident}			{ return (int)Tokens.ident; }
{Number}		{ return (int)Tokens.number; }