using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using BrightScriptDebug.Compiler;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Services.Parser
{
    public class ParserService : IParserService
    {
        private volatile bool _running = false;
        private PipeStream _stream;
        private StreamWriter _writer;
        private CancellationTokenSource _cancellationToken;
        private readonly IEventAggregator _eventAggregator;

        public ParserService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public void Start(int port)
        {
            if (!_running)
            {
                Port = port;

                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);
                _cancellationToken = new CancellationTokenSource();

                _eventAggregator.GetEvent<LogEvent>().Subscribe(ProcessLog, ThreadOption.BackgroundThread);

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

                                parser.BacktraceProcessed += PublishBacktrace;
                                parser.VariablesProcessed += PublishVariables;
                                parser.DebugPorcessed += PublishDebug;
                                parser.AppCloseProcessed += PublishAppClose;
                                parser.AppOpenProcessed += PublishAppOpen;

                                try
                                {
                                    parser.Parse();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                parser.BacktraceProcessed -= PublishBacktrace;
                                parser.VariablesProcessed -= PublishVariables;
                                parser.DebugPorcessed -= PublishDebug;
                                parser.AppCloseProcessed -= PublishAppClose;
                                parser.AppOpenProcessed -= PublishAppOpen;
                            }

                            Task.Delay(100).Wait();
                        }

                        if(_running && scanner.Restart)
                            Console.WriteLine("Restart Scanner {0}", port);
                    }

                }, _cancellationToken.Token);
            }
        }

        private void PublishBacktrace(List<BacktraceModel> trace)
        {
            _eventAggregator.GetEvent<BacktraceEvent>().Publish(trace);
            _eventAggregator.GetEvent<DebugEvent>().Publish(false);
        }

        private void PublishVariables(List<VariableModel> vars)
        {
            _eventAggregator.GetEvent<VariablesEvent>().Publish(vars);
            _eventAggregator.GetEvent<DebugEvent>().Publish(false);
        }

        private void PublishDebug()
        {
            _eventAggregator.GetEvent<DebugEvent>().Publish(true);
        }

        private void PublishAppClose()
        {
            _eventAggregator.GetEvent<AppCloseEvent>().Publish(null);
            _eventAggregator.GetEvent<DebugEvent>().Publish(false);
        }

        private void PublishAppOpen()
        {
            _eventAggregator.GetEvent<AppOpenEvent>().Publish(null);
            _eventAggregator.GetEvent<DebugEvent>().Publish(false);
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
                _eventAggregator.GetEvent<LogEvent>().Unsubscribe(ProcessLog);
                _cancellationToken.Cancel(true);
                _stream.Dispose();
                _stream = null;
            }
        }

        public int Port { get; private set; }
    }
}