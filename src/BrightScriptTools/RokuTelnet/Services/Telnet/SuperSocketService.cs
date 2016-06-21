using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RokuTelnet.Services.Telnet.Utils;
using SuperSocket.ClientEngine;

namespace RokuTelnet.Services.Telnet
{
    public class SuperSocketService : ITelenetService
    {
        private EasyClient _client;

        public async Task<bool> Connect(string ip, int port)
        {
            _client = new EasyClient();

            // Initialize the client with the receive filter and request handler
            _client.Initialize(
                new RokuReceiverFilter(Encoding.ASCII, new RokuStringParser()),
                (request) =>
                {
                    Console.WriteLine(request.Body);
                    if (Log != null)
                    {
                        var parts = request.Body.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        parts.ToList().ForEach(s => Log(s));
                    }
                });

            _client.Closed += (s, e) =>
            {
                if (Close != null)
                    Close();
            };

            // Connect to the server
            return await _client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        public void Disconnect()
        {
            _client.Close();
        }

        public event Action<string> Log;
        public event Action Close;

        public void Send(string cmd)
        {
            if (_client != null && _client.IsConnected)
                _client.Send(new ArraySegment<byte>(Encoding.ASCII.GetBytes(cmd + Environment.NewLine)));
        }
    }
}