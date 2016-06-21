using System;
using System.Threading.Tasks;

namespace RokuTelnet.Services.Telnet
{
    public interface ITelenetService
    {
        Task<bool> Connect(string ip, int port);
        void Disconnect();
        event Action<string> Log;
        event Action Close;

        void Send(string cmd);
    }
}