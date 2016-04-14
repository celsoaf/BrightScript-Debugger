
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
Typs			(string|inte(rface|ger)|object|double|float|b(oolean|rsub)|void)

// BrightScript Statements.txt
// Keywords		(s(t(op|ep)|ub)|i(n|f)|t(hen|o)|dim|print|e(nd(if|(sub|f(or|unction)|while))?|lse(if)?|xit(for|while)?|ach)|f(or|unction)|as|while|re(turn|m)|goto)

// BrightScript Functions.txt
BuiltInFuncs	(GetLastRun(RuntimeError|CompileError)|R(nd|un)|Box|Type)
GlobalFuncs		(Re(adAsciiFile|bootSystem)|GetInterface|MatchFiles|Sleep|C(opyFile|reate(Directory|Object))|Delete(Directory|File)|UpTime|FormatDrive|ListDir|W(ait|riteAsciiFile))
StringFuncs		(Right|Mid|Str(i(ng(i)?)?)?|Chr|Instr|UCase|Val|Asc|L(Case|e(n|ft)))
MathFuns		(S(in|qr|gn)|C(sng|dbl|os)|Tan|Int|Exp|Fix|Log|A(tn|bs))

DotChr			[^\r\n]

//Opr				\.|\(|\)|\[|\]|\{|\}|=|>=|<zz|>|<|<>|\+|-|\*|\/|\^|&|\b(?i:(And|Not|Or))\\b
bar				\| 
dot				\. 
semi			; 
colon			:
star			\* 
lt				< 
gt				> 
ltEqual			<= 
gtEqual			>=
notEqual		<> 
comma			\, 
slash			\/ 
lBrac			\[ 
rBrac			\] 
lPar			\(
rPar			\) 
lBrace			\{	
rBrace			\}
equal			=
plus			\+
minus			-
questionMark	\?

bsIf			if
bsElse			else
bsFor			for
bsTo			to
bsEach			each
bsStep			step
bsIn			in
bsWhile			while
bsNot			not
bsTrue			true
bsFalse			false
bsInvalid		invalid
bsM				m
bsStop			stop
bsReturn		return
print			print 

Number			{Num}|{Real}
Cmnt			\'{DotChr}*
Str				\"{DotChr}*\"
Funcs			{BuiltInFuncs}|{GlobalFuncs}|{StringFuncs}|{MathFuns}


// =============================================================
%%  // Start of rules
// =============================================================

/*
LINE_NUM 		|
NEXT  			|
OBJFUN			|
POS 			|
RND 			|
TAB				{ return (int)Tokens.reserved; }
*/

{Cmnt}			{ return (int)Tokens.comment; }

SUB				{ return (int)Tokens.bsSub; }
FUNCTION		{ return (int)Tokens.bsFunction; }
END				{ return (int)Tokens.bsEnd; }
AS				{ return (int)Tokens.bsAs; }

{bar}			{ return (int)Tokens.bar; }
{dot}			{ return (int)Tokens.dot; }
{semi}			{ return (int)Tokens.semi; }
{colon}			{ return (int)Tokens.colon; }
{star}			{ return (int)Tokens.star; }
{lt}			{ return (int)Tokens.lt; }
{gt}			{ return (int)Tokens.gt; }
{ltEqual}		{ return (int)Tokens.ltEqual; }
{gtEqual}		{ return (int)Tokens.gtEqual; }
{notEqual}		{ return (int)Tokens.notEqual; }
{comma}			{ return (int)Tokens.comma; }
{slash}			{ return (int)Tokens.slash; }
{lBrac}			{ return (int)Tokens.lBrac; }
{rBrac}			{ return (int)Tokens.rBrac; }
{lPar}			{ return (int)Tokens.lPar; }
{rPar}			{ return (int)Tokens.rPar; }
{lBrace}		{ return (int)Tokens.lBrace; }
{rBrace}		{ return (int)Tokens.rBrace; }
{Eol}			{ return (int)Tokens.Eol;}
{equal}			{ return (int)Tokens.equal;}
{plus}			{ return (int)Tokens.plus;}
{minus}			{ return (int)Tokens.minus;}

{bsTrue}		{ return (int)Tokens.bsTrue;}
{bsFalse}		{ return (int)Tokens.bsFalse;}
{bsInvalid}		{ return (int)Tokens.bsInvalid;}
{bsM}			{ return (int)Tokens.bsM; }
{bsStop}		{ return (int)Tokens.bsStop; }
{bsReturn}		{ return (int)Tokens.bsReturn; }

{bsIf}			{ return (int)Tokens.bsIf;}
{bsElse}		{ return (int)Tokens.bsElse;}
{bsFor}			{ return (int)Tokens.bsFor;}
{bsTo}			{ return (int)Tokens.bsTo;}
{bsEach}		{ return (int)Tokens.bsEach;}
{bsStep}		{ return (int)Tokens.bsStep;}
{bsIn}			{ return (int)Tokens.bsIn;}
{bsWhile}		{ return (int)Tokens.bsWhile;}
{bsNot}			{ return (int)Tokens.bsNot;}

{print}			{ return (int)Tokens.bsPrint; }
{questionMark}	{ return (int)Tokens.questionMark; }

//{Keywords}	{ return (int)Tokens.keyword; }
{Typs}			{ return (int)Tokens.bsType; }
{Str}			{ return (int)Tokens.bsStr; }
{Funcs}			{ return (int)Tokens.bsFuncs; }
{Number}		{ return (int)Tokens.bsNumber; }
{Ident}			{ return (int)Tokens.bsIdent; }
//{Opr}			{ return (int)Tokens.opr; }
