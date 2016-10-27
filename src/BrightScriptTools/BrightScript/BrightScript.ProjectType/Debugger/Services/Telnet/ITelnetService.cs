using System;
using System.Threading.Tasks;

namespace BrightScript.Debugger.Services.Telnet
{
    public interface ITelnetService
    {
        Task<bool> Connect(string ip, int port);
        void Disconnect();
        event Action<string> Log;
        event Action Close;

        void Send(string cmd);

        int Port { get; }
    }
}