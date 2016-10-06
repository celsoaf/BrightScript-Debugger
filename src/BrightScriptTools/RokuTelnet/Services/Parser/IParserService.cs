namespace RokuTelnet.Services.Parser
{
    public interface IParserService
    {
        void Start(int port);
        void Stop();

        int Port { get; }
    }
}