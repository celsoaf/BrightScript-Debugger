
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan

%token keyword ident number str cmnt type funcs reserved opr
       bar "|", dot ".", semi ";", star "*", lt "<", gt ">", 
       comma ",", slash "/", lBrac "[", rBrac "]", lPar "(",
       rPar ")", lBrace "{", rBrace "}"
%token sub "Sub", function "Function"
%token comment "'"

%token maxParseToken EOL comment errTok repErr

%%

Program
    : SubSection 
    | error
    ;

SubSection
	: sub
	| error 
	;

%%