using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;

namespace RokuTelnet.Services.Telnet
{
    public class TcpService : ITelnetService
    {
        private TcpClient _client;
        protected StreamReader _reader;
        protected StreamWriter _writer;
        private volatile bool _running = false;
        private IEventAggregator _eventAggregator;
        private string _ip;

        public TcpService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task<bool> Connect(string ip, int port)
        {
            _ip = ip;
            Port = port;

            try
            {
                _client = new TcpClient();
                _client.Connect(ip, port);

                _reader = new StreamReader(_client.GetStream());
                _writer = new StreamWriter(_client.GetStream());

                Task.Factory.StartNew(TransportLoop);

                _running = true;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void TransportLoop()
        {
            while (_running)
            {
                string line = GetLine();
                if (line == null)
                    break;

                line = line.TrimEnd();

                try
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        Log?.Invoke(line);
                }
                catch (ObjectDisposedException)
                {
                    Debug.Assert(!_running);
                    break;
                }
            }
        }

        private string GetLine()
        {
            try
            {
                while (_client.Available == 0)
                    Thread.Sleep(1000);

                if (_client.Available > 0)
                {
                    byte[] buffer = new byte[_client.Available];
                    Task<int> task = _client.GetStream().ReadAsync(buffer, 0, _client.Available);

                    return System.Text.Encoding.Default.GetString(buffer);
                }

                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
            // I have seen the StreamReader throw both an ObjectDisposedException (which makes sense) and a NullReferenceException
            // (which seems like a bug) after it is closed. Since we have no exception back stop here, we are catching all exceptions
            // here (we don't want to crash VS).
            catch
            {
                Debug.Assert(!_running, "Exception throw from ReadLine when we haven't quit yet");
                return null;
            }
        }

        public void Disconnect()
        {
            _running = false;
            _client.Close();
        }

        public event Action<string> Log;
        public event Action Close;
        public void Send(string cmd)
        {
            _writer.WriteLine(cmd);
            _writer.Flush();
        }

        public int Port { get; private set; }
    }
}