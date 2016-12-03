using System.Net;

namespace BrightScript.Debugger.Transport
{
    public interface ITransport
    {
        void Init(IPEndPoint endPoint, ITransportCallback transportCallback);
        void Send(string cmd);
        void Close();
        bool IsClosed { get; }
    }
}