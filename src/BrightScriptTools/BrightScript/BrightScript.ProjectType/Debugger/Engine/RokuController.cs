using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Services.Parser.Utils;
using BrightScript.Debugger.Transport;
using BrightScript.Loggger;

namespace BrightScript.Debugger.Engine
{
    internal class RokuController : IRokuController
    {
        private readonly IPEndPoint _endPoint;
        private ITransport _transport;

        private ConcurrentBag<CommandModel> _operations = new ConcurrentBag<CommandModel>();

        private readonly Dictionary<DebuggerCommandEnum, string> _injectStrings;
        private DebuggerCommandEnum? _lasCommand;

        public RokuController(IPEndPoint endPoint)
        {
            _endPoint = endPoint;

            _injectStrings = new Dictionary<DebuggerCommandEnum, string>();
            //_injectStrings.Add(DebuggerCommandEnum.bt, "Backtrace: ");
            _injectStrings.Add(DebuggerCommandEnum.var, "Local Variables: ");
            _injectStrings.Add(DebuggerCommandEnum.list, "Current Function: ");
        }

        public event Action<string> OnOutput;
        public event Action<List<ThreadContext>> OnBackTrace;
        public event Action<List<SimpleVariableInformation>> OnVariables;
        public event Action<int> BreakModeEvent;
        public event Action RunModeEvent;
        public event Action ProcessExitEvent;

        public void Connect()
        {
            _transport = new TcpTransport();
            _transport.Init(_endPoint, this);
        }

        public void Close()
        {
            _transport?.Close();
        }

        public async Task<Res> CmdAsync<Res>(CommandModel cmd) where Res : class
        {
            if (cmd.ResultType != CommandType.NoResult)
                _operations.Add(cmd);

            if (cmd.Cmd != DebuggerCommandEnum.custom)
            {
                _transport.Send($"{cmd.Cmd} {cmd.Arg}");
                OnOutput?.Invoke($"<--{cmd.Cmd} {cmd.Arg}");
            }
            else
            {
                _transport.Send(cmd.Arg);
                OnOutput?.Invoke($"<--{cmd.Arg}");
            }
            _lasCommand = cmd.Cmd;

            if (cmd.ResultType != CommandType.NoResult)
            {
                cmd.Wait();

                return cmd.Result as Res;
            }

            return null;
        }

        private void DispatchCommands(CommandType type, object result = null)
        {
            _operations//.ToList()
                .Where(c => c.ResultType == type)
                .ToList()
                .ForEach(c =>
                {
                    c.Result = result;
                    c.Set();
                });
        }

        private volatile bool _initialized = false;
        public void OnStdOutLine(string line)
        {
            if (line.Length == 0) return;
            if (!_initialized && !line.Contains("------ Running dev '")) return;

            if (!_initialized)
            {
                var index = line.LastIndexOf("------ Running dev '", StringComparison.Ordinal);
                if (index > 0)
                {
                    line = Environment.NewLine + line.Substring(index);
                    if (line.Contains("------ Compiling dev '")) return;
                    _initialized = true;
                }
            }

            if (_lasCommand.HasValue)
            {
                if (_injectStrings.ContainsKey(_lasCommand.Value))
                    line = _injectStrings[_lasCommand.Value] + Environment.NewLine + line;


                _lasCommand = null;
            }

            OnOutput?.Invoke(line);

            DispatchGeneric(line);

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var sw = new StreamWriter(ms))
                    {
                        sw.WriteLine(line);
                        sw.Flush();
                        ms.Position = 0;

                        // parse input args, and open input file
                        var scanner = new TelnetScanner(ms);
                        scanner.ErrorPorcessed += PublishError;

                        var parser = new BrightScriptDebug.Compiler.Parser(scanner);

                        parser.BacktraceProcessed += ParserOnBacktraceProcessed;
                        parser.VariablesProcessed += ParserOnVariablesProcessed;
                        parser.DebugPorcessed += ParserOnDebugPorcessed;
                        parser.AppCloseProcessed += ParserOnAppCloseProcessed;
                        parser.AppOpenProcessed += ParserOnAppOpenProcessed;
                        parser.CurrentFunctionProcessed += ParserOnCurrentFunctionProcessed;
                        parser.StepPorcessed += ParserOnStepPorcessed;

                        try
                        {
                            parser.Parse();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        parser.BacktraceProcessed -= ParserOnBacktraceProcessed;
                        parser.VariablesProcessed -= ParserOnVariablesProcessed;
                        parser.DebugPorcessed -= ParserOnDebugPorcessed;
                        parser.AppCloseProcessed -= ParserOnAppCloseProcessed;
                        parser.AppOpenProcessed -= ParserOnAppOpenProcessed;
                        parser.CurrentFunctionProcessed -= ParserOnCurrentFunctionProcessed;
                        parser.StepPorcessed += ParserOnStepPorcessed;
                    }
                }
            }
            catch (Exception ex)
            {
                LiveLogger.WriteLine(ex.Message);
            }
        }

        private void DispatchGeneric(string line)
        {
            var debug = "Brightscript Debugger>";
            var value = line;
            if (value.Contains(debug))
                value = value.Remove(value.LastIndexOf(debug));

            value = value.Trim();

            DispatchCommands(CommandType.Print, value);
            DispatchCommands(CommandType.Command, value);
        }

        private void ParserOnCurrentFunctionProcessed(List<string> list)
        {

        }

        private void ParserOnStepPorcessed()
        {
            DispatchCommands(CommandType.Step);
        }

        private void ParserOnVariablesProcessed(List<VariableModel> variableModels)
        {
            var vars = variableModels.Select(v =>
                    new SimpleVariableInformation(v.Ident, false, v.Value))
                .ToList();

            OnVariables?.Invoke(vars);

            DispatchCommands(CommandType.Variables, vars);
        }

        private async void ParserOnAppOpenProcessed()
        {
            RunModeEvent?.Invoke();
        }

        private void ParserOnBacktraceProcessed(List<BacktraceModel> backtraceModels)
        {
            var backtrace = backtraceModels.Select(f =>
                    new ThreadContext(
                        null,
                        new MITextPosition(RokuPathToWindowsPath(f.File), (uint)f.Line),
                        f.Function,
                        (uint)f.Position,
                        null))
                .ToList();

            DispatchCommands(CommandType.Backtrace, backtrace);

            OnBackTrace?.Invoke(backtrace);
        }

        internal static string RokuPathToWindowsPath(string unixPath)
        {
            return unixPath
                        .Replace("pkg:/", "")
                        .Replace('/', '\\');
        }

        private async void ParserOnDebugPorcessed()
        {
            BreakModeEvent?.Invoke(0);
        }

        private void ParserOnAppCloseProcessed()
        {
            ProcessExitEvent?.Invoke();
        }

        private void PublishError(string error)
        {
            LiveLogger.WriteLine(error);
        }

        public void OnDebuggerProcessExit(string exitCode)
        {
            throw new NotImplementedException();
        }
    }
}