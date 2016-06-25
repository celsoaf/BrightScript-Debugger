using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using BrightScriptDebug.Compiler;
using Prism.Events;
using RokuTelnet.Events;

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


        public void Start()
        {
            if (!_running)
            {
                _running = true;
                _stream = new PipeStream();
                _writer = new StreamWriter(_stream);
                _cancellationToken = new CancellationTokenSource();

                _eventAggregator.GetEvent<LogEvent>().Subscribe(ProcessLog);

               Task.Factory.StartNew(() =>
                {
                    while (_running)
                    {
                        Task.Delay(1000).Wait();

                        // parse input args, and open input file
                        var scanner = new Scanner(_stream);

                        var parser = new BrightScriptDebug.Compiler.Parser(scanner);

                        parser.Parse();
                    }
                }, _cancellationToken.Token);
            }
        }

        private void ProcessLog(string msg)
        {
            _writer.WriteLine(msg);
            _writer.Flush();
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