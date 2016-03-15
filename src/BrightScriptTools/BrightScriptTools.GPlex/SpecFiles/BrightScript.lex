
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

OneLineCmnt		'{DotChr}*


// =============================================================
%%  // Start of rules
// =============================================================

function		{ return (int)Tokens.function; }
{Ident}			{ return (int)Tokens.ident; }
{Number}		{ return (int)Tokens.number; }