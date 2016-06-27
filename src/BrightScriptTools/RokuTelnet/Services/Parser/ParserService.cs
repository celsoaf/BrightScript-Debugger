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
        private object _locker = new object();
        private bool _hasContent = false;

        public ParserService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public void Start()
        {
            if (!_running)
            {
                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);
                _cancellationToken = new CancellationTokenSource();

                _eventAggregator.GetEvent<LogEvent>().Subscribe(ProcessLog, ThreadOption.BackgroundThread);

                Task.Factory.StartNew(() =>
                {
                    // parse input args, and open input file
                    var scanner = new TelnetScanner(_stream);

                    while (_running)
                    {
                        Task.Delay(100).Wait();

                        if (_hasContent)
                        {
                            lock (_locker)
                            {
                                if (_hasContent)
                                {
                                    var parser = new BrightScriptDebug.Compiler.Parser(scanner);

                                    parser.BacktraceProcessed += PublishBacktrace;
                                    parser.VariablesProcessed += PublishVariables;
                                    parser.DebugPorcessed += PublishDebug;
                                    parser.AppCloseProcessed += PublishAppClose;
                                    parser.AppOpenProcessed += PublishAppOpen;

                                    parser.Parse();

                                    parser.BacktraceProcessed -= PublishBacktrace;
                                    parser.VariablesProcessed -= PublishVariables;
                                    parser.DebugPorcessed -= PublishDebug;
                                    parser.AppCloseProcessed -= PublishAppClose;
                                    parser.AppOpenProcessed -= PublishAppOpen;

                                    _hasContent = false;
                                }
                            }
                        }
                    }
                }, _cancellationToken.Token);
            }
        }

        private void PublishBacktrace(List<BacktraceModel> trace)
        {
            _eventAggregator.GetEvent<BacktraceEvent>().Publish(trace);
        }

        private void PublishVariables(List<VariableModel> vars)
        {
            _eventAggregator.GetEvent<VariablesEvent>().Publish(vars);
        }

        private void PublishDebug()
        {
            _eventAggregator.GetEvent<DebugEvent>().Publish(true);
        }

        private void PublishAppClose()
        {
            _eventAggregator.GetEvent<AppCloseEvent>().Publish(null);
        }

        private void PublishAppOpen()
        {
            _eventAggregator.GetEvent<AppOpenEvent>().Publish(null);
        }

        private void ProcessLog(string msg)
        {
            lock (_locker)
            {
                _writer.WriteLine(msg);
                _writer.Flush();

                _hasContent = true;
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
    }
}