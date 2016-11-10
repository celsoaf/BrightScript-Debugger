using System;
using System.Collections.Generic;
using System.Linq;
using BrightScriptDebug.GPlex;
using BrightScriptDebug.GPlex.Parser;
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
            Console.WriteLine(@"ProcessCurrentFunction");

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
            Console.WriteLine(@"ProcessBacktrace");

            var stack = new List<BacktraceModel>();

            while (true)
            {
                Scanner.yylex();
                var trace = ((Scanner)Scanner).yytext;

                Scanner.yylex();
                Scanner.yylex();
                var file = ((Scanner)Scanner).yytext;

                if(string.IsNullOrWhiteSpace(trace) && string.IsNullOrWhiteSpace(file))
                    break;

                stack.Add(BuildBacktraceModel(trace, file));

                Scanner.yylex();
                if (trace.StartsWith("#0"))
                    break;
            }

            BacktraceProcessed?.Invoke(stack);
        }

        private BacktraceModel BuildBacktraceModel(string trace, string file)
        {
            try
            {
                var pos = int.Parse(trace.Substring(1, 3));
                var func = trace.Substring(4);

                var colonIdx = file.IndexOf(":");
                var lParIdx = file.IndexOf("(");
                var rParIdx = file.IndexOf(")");

                string f = string.Empty;
                int l = -1;
                if (colonIdx >= 0 && lParIdx >= 0 && rParIdx >= 0)
                {
                    f = file.Substring(colonIdx + 2, lParIdx - colonIdx - 2);
                    var ls = file.Substring(lParIdx + 1, rParIdx - lParIdx - 1);
                    l = int.Parse(ls);
                }
                return new BacktraceModel
                {
                    Position = pos,
                    Function = func,
                    File = f,
                    Line = l
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<BacktraceModel> _stack = new List<BacktraceModel>();
        public void ProcessBacktraceLine()
        {
            Console.WriteLine(@"ProcessBacktraceLine");

            var tc = ((TelnetScanner) Scanner);

            var trace = tc.GetToken(3);
            var file = tc.GetToken(1);

            var model = BuildBacktraceModel(trace, file);
            _stack.Add(model);

            if (trace.StartsWith("#0"))
            {
                BacktraceProcessed?.Invoke(_stack.ToList());
                _stack.Clear();
            }
        }

        public event Action<List<VariableModel>> VariablesProcessed;

        public void ProcessVariables()
        {
            Console.WriteLine(@"ProcessVariables");

            var dic = new List<VariableModel>();

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
                        dic.Add(new VariableModel { Ident = key, Value = value });
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

        public void ProcessStepResponse()
        {
            Console.WriteLine(@"ProcessSetpResponse");
        }

        public event Action DebugPorcessed;

        public void ProcessDebug()
        {
            Console.WriteLine(@"ProcessDebug");

            DebugPorcessed?.Invoke();
        }

        public event Action AppCloseProcessed;

        public void ProcessAppClose()
        {
            Console.WriteLine(@"ProcessAppClose");

            AppCloseProcessed?.Invoke();
        }

        public event Action AppOpenProcessed;

        public void ProcessAppOpen()
        {
            Console.WriteLine(@"ProcessAppOpen");

            AppOpenProcessed?.Invoke();
        }
    }
}