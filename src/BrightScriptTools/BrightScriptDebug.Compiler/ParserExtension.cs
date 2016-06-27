using System;
using System.Collections.Generic;
using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptDebug.Compiler
{
    public partial class Parser : ShiftReduceParser<int, LexSpan>
    {
        public Parser(AbstractScanner<int, LexSpan> scanner)
            : base(scanner)
        {

        }

        public void ProcessCurrentFunction()
        {
            var lines = new List<string>();

            while (Scanner.yylex() == (int)Tokens.dgCodeLine)
            {
                lines.Add(((Scanner)Scanner).yytext);
                Scanner.yylex();
            }
            var stop = ((Scanner)Scanner).yytext;
            Scanner.yylex();
            Scanner.yylex();
            Scanner.yylex();
        }

        public class BacktraceModel
        {
            public int Position { get; set; }
            public string Function { get; set; }
            public string File { get; set; }
            public int Line { get; set; }
        }

        public void ProcessBacktrace()
        {
            var stack = new List<BacktraceModel>();

            while (true)
            {
                Scanner.yylex();
                var trace = ((Scanner)Scanner).yytext;
                var pos = int.Parse(trace.Substring(1, 3));
                var func = trace.Substring(4);

                Scanner.yylex();
                Scanner.yylex();
                var file = ((Scanner)Scanner).yytext;
                var colonIdx = file.IndexOf(":");
                var lParIdx = file.IndexOf("(");
                var rParIdx = file.IndexOf(")");

                var f = file.Substring(colonIdx + 2, lParIdx - colonIdx - 2);
                var ls = file.Substring(lParIdx + 1, rParIdx - lParIdx - 1);
                var l = int.Parse(ls);

                stack.Add(new BacktraceModel
                {
                    Position = pos,
                    Function = func,
                    File = f,
                    Line = l
                });

                Scanner.yylex();
                if(trace.StartsWith("#0"))
                    break;
            }   
        }

        public void ProcessBacktraceLine()
        {

        }

        public void ProcessVariables()
        {
            var dic = new Dictionary<string, string>();

            int last;
            int curr = 0;
            string key = null;
            string value = null;
            do
            {
                last = curr;
                curr = Scanner.yylex();

                if (curr == (int)Tokens.Eol)
                {
                    if (key != null)
                        dic.Add(key, value);
                    key = value = null;
                }
                else if (key == null)
                    key = ((Scanner)Scanner).yytext;
                else if (value == null)
                    value = ((Scanner)Scanner).yytext;
                else
                    value += " " + ((Scanner)Scanner).yytext;
            } while (curr != last || curr != (int)Tokens.Eol);
        }

        public void ProcessDebug()
        {
            Console.WriteLine(@"Debug");
        }

        public void ProcessAppClose()
        {
            Console.WriteLine(@"App Closed");
        }

        public void ProcessAppOpen()
        {
            Console.WriteLine(@"App Open");
        }
    }
}