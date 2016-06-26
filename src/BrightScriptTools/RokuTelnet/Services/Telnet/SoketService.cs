﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RokuTelnet.Services.Telnet
{
    public class SoketService : ITelenetService
    {
        private Socket _client;
        private volatile bool _running = false;

        public async Task<bool> Connect(string ip, int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), 8085);

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
            while (_running)
            {
                var bytes = new Byte[256];
                String responseData = String.Empty;
                int bytesRead = 0;
                do
                {
                    if (_client.Poll(1000, SelectMode.SelectRead))
                        bytesRead = _client.Receive(bytes, _client.Available > bytes.Length ? bytes.Length : _client.Available, SocketFlags.None);
                    else
                        bytesRead = 0;

                    responseData += Encoding.ASCII.GetString(bytes, 0, bytesRead);
                } while (bytesRead == bytes.Length);

                if (!string.IsNullOrEmpty(responseData))
                    Log?.Invoke(responseData);

                Thread.Sleep(1000);
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
    }
}