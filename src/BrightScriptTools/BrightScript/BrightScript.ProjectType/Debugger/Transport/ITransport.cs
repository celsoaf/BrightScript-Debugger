namespace BrightScript.Debugger.Transport
{
    public interface ITransport
    {
        void Init(string ip, int port, ITransportCallback transportCallback);
        void Send(string cmd);
        void Close();
        bool IsClosed { get; }
    }
}