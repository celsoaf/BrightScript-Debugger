
// =============================================================

%using System.Collections;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.GPlex.Lexer

%visibility public

%option babel, caseinsensitive, stack, classes, minimize, parser, summary, unicode, verbose, persistbuffer, noembedbuffers, out:Scanner.cs

// =============================================================
// =============================================================

Eol             (\r\n?|\n)
Space           [ \t]
Ident           [a-zA-Z_][a-zA-Z0-9_]*
Number          [0-9]+

DotChr			[^\r\n]

OneLineCmnt		\'{DotChr}*


// =============================================================
%%  // Start of rules
// =============================================================

INVALID  		|
FOR  			|
POS 			|
AND  			|
PRINT  			|
LINE_NUM 		|
OR  			|
GOTO  			|
REM 			|
EACH  			|
IF  			|
RETURN 			|
NEXT  			|
NOT  			|
STEP 			|
DIM  			|
THEN  			|
STOP 			|
ELSE  			|
TO 				|
TAB				|
END 			|
ENDFOR 			|
OBJFUN			|
TYPE 			|
RND 			|
TRUE			|
FALSE 			|
CREATEOBJECT	| 
WHILE			|
ENDWHILE 		|
EXITWHILE 		|
ENDSUB			|
SUB 			|
FUNCTION 		|
EACH			|
EXIT			|
ENDFUNCTION		|
ENDIF			{ return (int)Tokens.keyword; }
{Ident}			{ return (int)Tokens.ident; }
{Number}		{ return (int)Tokens.number; }