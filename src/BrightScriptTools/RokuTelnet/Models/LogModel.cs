namespace RokuTelnet.Models
{
    public class LogModel
    {
        public LogModel(int port, string message)
        {
            Port = port;
            Message = message;
        }

        public int Port { get; private set; }
        public string Message { get; private set; }
    }
}