
%output=..\..\SpecFiles\Parser.cs
 
%using System.Collections;
%using BrightScriptTools.GPlex;
%using BrightScriptTools.GPlex.Parser;

%namespace BrightScriptTools.Compiler

%scanbasetype ScanBase
%visibility public
%partial

%YYLTYPE LexSpan

%token	bar dot semi star lt gt comma slash lBrac rBrac lPar rPar lBrace rBrace

%token bsIdent bsNumber bsStr bsCmnt bsFuncs bsType bsAs
%token bsSub, bsFunction, bsEnd

%token maxParseToken EOL comment errTok repErr

%%

Program
    : FunctionLst EOF
    ;

FunctionLst
	: FunctionImpl FunctionLst
	| /* Empty */
	;

FunctionImpl
	: SubSection 
	| FunctionSection
	;

FunctionSection
	: bsFunction bsIdent lPar ParamLst rPar AsBlock bsEnd bsFunction
	;

SubSection
	: bsSub bsIdent lPar ParamLst rPar bsEnd bsSub 
	;

ParamLst
	: Param ResParam
	| /* Empty */
	;

ResParam
	: comma Param ResParam
	| /* Empty */
	;

Param
	: bsIdent AsBlock
	;

AsBlock
	: bsAs bsType
	| /* Empty */
	;

%%