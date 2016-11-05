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
        private BrightScriptDebug.Compiler.Parser _parser;
        private volatile bool _initialized = false;
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public async Task Start(int port)
        {
            if (!_running)
            {
                Port = port;

                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);

                _thread = new Thread(Run);
                _thread.Start();

                _writer.WriteLine("");
                _writer.Flush();

                _manualResetEvent.WaitOne();
                _manualResetEvent.Reset();

                _writer.WriteLine("");
                _writer.Flush();

                _manualResetEvent.WaitOne();
            }
        }

        private void Run()
        {
            while (_running)
            {
                try
                {
                    // parse input args, and open input file
                    var scanner = new TelnetScanner(_stream);
                    scanner.ErrorPorcessed += PublishError;

                    while (_running && !scanner.Restart)
                    {
                        if (_stream.Length > 0)
                        {
                            _parser = new BrightScriptDebug.Compiler.Parser(scanner);

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

                        if (!_initialized)
                            _manualResetEvent.Set();

                        Thread.Sleep(100);
                    }

                    if (_running && scanner.Restart)
                        Console.WriteLine("Restart Scanner {0}", Port);
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

        private void PublishError(string error)
        {
            ErrorProcessed?.Invoke(error);
        }

        public void ProcessLog(LogModel log)
        {
            if (_running && log.Port == Port)
            {
                var msg = log.Message;
                if (!_initialized)
                {
                    _initialized = true;

                    var index = msg.LastIndexOf("------ Running dev '", StringComparison.Ordinal);
                    if (index > 0)
                    {
                        msg = Environment.NewLine + msg.Substring(index);
                    }
                }
                _writer.WriteLine(msg);
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
                _initialized = false;
            }
        }

        public int Port { get; private set; }

        public event Action<List<string>> CurrentFunctionProcessed;
        public event Action<List<BacktraceModel>> BacktraceProcessed;
        public event Action<List<VariableModel>> VariablesProcessed;
        public event Action DebugPorcessed;
        public event Action AppCloseProcessed;
        public event Action AppOpenProcessed;
        public event Action<string> ErrorProcessed;
    }
}