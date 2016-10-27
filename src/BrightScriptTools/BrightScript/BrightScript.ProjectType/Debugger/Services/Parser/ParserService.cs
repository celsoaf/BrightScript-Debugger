using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Services.Parser.Utils;

namespace BrightScript.Debugger.Services.Parser
{
    public class ParserService : IParserService
    {
        private volatile bool _running = false;
        private PipeStream _stream;
        private StreamWriter _writer;
        private CancellationTokenSource _cancellationToken;


        public void Start(int port)
        {
            if (!_running)
            {
                Port = port;

                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);
                _cancellationToken = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {
                    while (_running)
                    {
                        // parse input args, and open input file
                        var scanner = new TelnetScanner(_stream);
                        scanner.ErrorPorcessed += PublishError;

                        while (_running && !scanner.Restart)
                        {
                            if (_stream.Length > 0)
                            {
                                var parser = new BrightScriptDebug.Compiler.Parser(scanner);

                                parser.BacktraceProcessed += ParserOnBacktraceProcessed;
                                parser.VariablesProcessed += ParserOnVariablesProcessed;
                                parser.DebugPorcessed += ParserOnDebugPorcessed;
                                parser.AppCloseProcessed += ParserOnAppCloseProcessed;
                                parser.AppOpenProcessed += ParserOnAppOpenProcessed;
                                parser.CurrentFunctionProcessed += ParserOnCurrentFunctionProcessed;

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
                            }

                            Task.Delay(100).Wait();
                        }

                        if (_running && scanner.Restart)
                            Console.WriteLine("Restart Scanner {0}", port);
                    }

                }, _cancellationToken.Token);
            }
        }

        private void ParserOnCurrentFunctionProcessed(List<string> list)
        {
            CurrentFunctionProcessed?.Invoke(list);
        }

        private void ParserOnAppOpenProcessed()
        {
            AppOpenProcessed?.Invoke();
        }

        private void ParserOnAppCloseProcessed()
        {
            AppCloseProcessed?.Invoke();
        }

        private void ParserOnDebugPorcessed()
        {
            DebugPorcessed?.Invoke();
        }

        private void ParserOnVariablesProcessed(List<VariableModel> variableModels)
        {
            VariablesProcessed?.Invoke(variableModels);
        }

        private void ParserOnBacktraceProcessed(List<BacktraceModel> backtraceModels)
        {
            BacktraceProcessed?.Invoke(backtraceModels);
        }

        private void PublishError()
        {
            //_eventAggregator.GetEvent<DebugEvent>().Publish(false);
        }

        private void ProcessLog(LogModel log)
        {
            if (log.Port == Port)
            {
                _writer.WriteLine(log.Message);
                _writer.Flush();
            }
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _cancellationToken.Cancel(true);
                _stream.Dispose();
                _stream = null;
            }
        }

        public int Port { get; private set; }

        public event Action<List<string>> CurrentFunctionProcessed;
        public event Action<List<BacktraceModel>> BacktraceProcessed;
        public event Action<List<VariableModel>> VariablesProcessed;
        public event Action DebugPorcessed;
        public event Action AppCloseProcessed;
        public event Action AppOpenProcessed;
    }
}