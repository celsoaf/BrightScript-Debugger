using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Services.Parser.Utils;
using BrightScriptDebug.Compiler;

namespace BrightScript.Debugger.Services.Parser
{
    public class ParserService : IParserService
    {
        private volatile bool _running = false;
        private PipeStream _stream;
        private StreamWriter _writer;
        private Thread _thread;
        private Scanner _scanner;
        private BrightScriptDebug.Compiler.Parser _parser;

        public void Start(int port)
        {
            if (!_running)
            {
                Port = port;

                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);

                _thread = new Thread(Run);
                _thread.Start();
            }
        }

        private void Run()
        {
            while (_running)
            {
                try
                {
                    // parse input args, and open input file
                    _scanner = new TelnetScanner(_stream);
                    _scanner.ErrorPorcessed += PublishError;

                    while (_running)
                    {
                        if (_stream.Length > 0)
                        {
                            _parser = new BrightScriptDebug.Compiler.Parser(_scanner);

                            _parser.BacktraceProcessed += ParserOnBacktraceProcessed;
                            _parser.VariablesProcessed += ParserOnVariablesProcessed;
                            _parser.DebugPorcessed += ParserOnDebugPorcessed;
                            _parser.AppCloseProcessed += ParserOnAppCloseProcessed;
                            _parser.AppOpenProcessed += ParserOnAppOpenProcessed;
                            _parser.CurrentFunctionProcessed += ParserOnCurrentFunctionProcessed;

                            try
                            {
                                _parser.Parse();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            _parser.BacktraceProcessed -= ParserOnBacktraceProcessed;
                            _parser.VariablesProcessed -= ParserOnVariablesProcessed;
                            _parser.DebugPorcessed -= ParserOnDebugPorcessed;
                            _parser.AppCloseProcessed -= ParserOnAppCloseProcessed;
                            _parser.AppOpenProcessed -= ParserOnAppOpenProcessed;
                            _parser.CurrentFunctionProcessed -= ParserOnCurrentFunctionProcessed;
                        }

                        Thread.Sleep(100);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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

        public void ProcessLog(LogModel log)
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