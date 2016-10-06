using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Services.Telnet
{
    public class SoketService : ITelnetService
    {
        private Socket _client;
        private volatile bool _running = false;
        private IEventAggregator _eventAggregator;
        private string _ip;

        public SoketService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task<bool> Connect(string ip, int port)
        {
            _ip = ip;
            Port = port;

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _client.Connect(remoteEP);
                _running = true;

                Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);

                return _client.Connected;
            }
            catch (Exception ex)
            {
                if (Log != null)
                    Log(ex.Message);

                return false;
            }
        }

        private void Run()
        {
            try
            {
                while (_running)
                {
                    var bytes = new Byte[256];
                    String responseData = String.Empty;
                    int bytesRead = 0;
                    do
                    {
                        if (_client.Poll(1000, SelectMode.SelectRead))
                            bytesRead = _client.Receive(bytes,
                                _client.Available > bytes.Length ? bytes.Length : _client.Available, SocketFlags.None);
                        else
                            bytesRead = 0;

                        responseData += Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    } while (bytesRead == bytes.Length);

                    if (!string.IsNullOrEmpty(responseData))
                        Log?.Invoke(responseData);

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_eventAggregator.GetEvent<DisconnectEvent>().Publish(null);

                //Task.Delay(1000).ContinueWith(t => _eventAggregator.GetEvent<ConnectEvent>().Publish(_ip));
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
            if (_client != null && _client.Connected)
            {
                byte[] msg = Encoding.ASCII.GetBytes(cmd + Environment.NewLine);

                _client.Send(msg);
            }
        }

        public int Port { get; private set; }
    }
}