using System;
using System.Collections.Generic;
using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;
using RokuTelnet.Models;

namespace BrightScriptDebug.Compiler
{
    public partial class Parser : ShiftReduceParser<int, LexSpan>
    {
        public Parser(AbstractScanner<int, LexSpan> scanner)
            : base(scanner)
        {

        }

        public event Action<List<string>> CurrentFunctionProcessed; 

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

            CurrentFunctionProcessed?.Invoke(lines);
        }

        public event Action<List<BacktraceModel>> BacktraceProcessed; 

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

            BacktraceProcessed?.Invoke(stack);
        }

        public event Action<Dictionary<string, string>> VariablesProcessed; 

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

            VariablesProcessed?.Invoke(dic);
        }

        public event Action DebugPorcessed; 

        public void ProcessDebug()
        {
            Console.WriteLine(@"Debug");

            DebugPorcessed?.Invoke();
        }

        public event Action AppCloseProcessed; 

        public void ProcessAppClose()
        {
            Console.WriteLine(@"App Closed");

            AppCloseProcessed?.Invoke();
        }

        public event Action AppOpenProcessed;

        public void ProcessAppOpen()
        {
            Console.WriteLine(@"App Open");

            AppOpenProcessed?.Invoke();
        }
    }
}