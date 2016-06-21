using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RokuTelnet.Telnet;

namespace RokuTelnet.Services.Telnet
{
    public class TelenetService : ITelenetService
    {
        private Client _client;
        private volatile bool _running = false;

        public async Task<bool> Connect(string ip, int port)
        {
            _running = true;

            _client = new Client(ip, port, CancellationToken.None);

            Task.Factory.StartNew(() =>
            {
                while (_running)
                {
                    _client.ReadAsync(TimeSpan.FromSeconds(1))
                        .ContinueWith(t =>
                        {
                            var txt = t.Result;
                            if (!string.IsNullOrEmpty(txt))
                            {
                                Console.WriteLine(txt);

                                if (Log != null)
                                {
                                    var parts = txt.Split(new string[] { Environment.NewLine },
                                        StringSplitOptions.None);
                                    parts.ToList().ForEach(s => Log(s));
                                }
                            }
                        })
                        .Wait();
                }
            }, TaskCreationOptions.LongRunning);

            return _client.IsConnected;
        }

        public void Disconnect()
        {
            _running = false;
        }

        public event Action<string> Log;
        public event Action Close;

        public void Send(string cmd)
        {
            if (_client != null && _client.IsConnected)
                _client.WriteLine(cmd +"\n");
        }
    }
}