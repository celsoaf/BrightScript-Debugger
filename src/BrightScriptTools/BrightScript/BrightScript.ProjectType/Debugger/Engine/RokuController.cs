using System;
using System.Net;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Transport;

namespace BrightScript.Debugger.Engine
{
    public class RokuController : IRokuController
    {
        private readonly IPEndPoint _endPoint;
        private ITransport _transport;

        public RokuController(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public event Action<string> OnOutput;

        public void Connect()
        {
            _transport = new TcpTransport();
            _transport.Init(_endPoint, this);
        }

        public void Close()
        {
            _transport?.Close();
        }

        public void OnStdOutLine(string line)
        {
            OnOutput?.Invoke(line);
        }

        public void OnDebuggerProcessExit(string exitCode)
        {
            throw new NotImplementedException();
        }
    }
}