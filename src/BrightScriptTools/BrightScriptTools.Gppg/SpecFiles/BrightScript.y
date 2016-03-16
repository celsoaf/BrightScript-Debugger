
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;

%namespace BrightScriptTools.Compiler

%scannertype Scanner
%scanbasetype ScanBase
%tokentype TokenEnum
%visibility public

%YYLTYPE LexSpan

%token keyword ident number 
       bar "|", dot ".", semi ";", star "*", lt "<", gt ">", 
       comma ",", slash "/", lBrac "[", rBrac "]", lPar "(",
       rPar ")", lBrace "{", rBrace "}"
%token comment "'"

%token maxParseToken EOL comment errTok repErr

%%

Program
    : DefinitionSection Rules
    | DefinitionSection RulesSection UserCodeSection
    ;