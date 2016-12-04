using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private readonly IThreadCache _threadCache;
        private ITransport _transport;

        public RokuController(IPEndPoint endPoint, IThreadCache threadCache)
        {
            _endPoint = endPoint;
            _threadCache = threadCache;
        }

        public event Action<string> OnOutput;
        public event Action BreakModeEvent;
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

            OnOutput?.Invoke(line);

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

        private void ParserOnCurrentFunctionProcessed(List<string> list)
        {

        }

        private void ParserOnStepPorcessed()
        {

        }

        private void ParserOnVariablesProcessed(List<VariableModel> variableModels)
        {

        }

        private async void ParserOnAppOpenProcessed()
        {
            RunModeEvent?.Invoke();
        }

        private void ParserOnBacktraceProcessed(List<BacktraceModel> backtraceModels)
        {
            _threadCache.SetStackFrames(
                0, 
                backtraceModels.Select(f => 
                    new ThreadContext(
                        null,
                        new MITextPosition(f.File, (uint) f.Line),
                        f.Function,
                        (uint) f.Position,
                        null))
                    .ToList()
            );
        }

        private async void ParserOnDebugPorcessed()
        {


            BreakModeEvent?.Invoke();
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