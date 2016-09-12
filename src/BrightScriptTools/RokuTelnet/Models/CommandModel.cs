namespace RokuTelnet.Models
{
    public class CommandModel
    {
        public CommandModel(int port, string command)
        {
            Port = port;
            Command = command;
        }

        public int Port { get; private set; }
        public string Command { get; private set; }
    }
}