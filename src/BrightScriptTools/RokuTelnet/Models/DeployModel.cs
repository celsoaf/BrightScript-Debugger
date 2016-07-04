namespace RokuTelnet.Models
{
    public class DeployModel
    {
        public DeployModel(string ip, string folder)
        {
            Ip = ip;
            Folder = folder;
        }

        public string Ip { get; set; }
        public string Folder { get; set; }
    }
}