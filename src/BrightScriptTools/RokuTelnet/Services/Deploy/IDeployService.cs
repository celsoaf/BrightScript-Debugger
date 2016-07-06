using System.Threading.Tasks;

namespace RokuTelnet.Services.Deploy
{
    public interface IDeployService
    {
        Task Deploy(string ip, string folder, string optionsFile);
    }
}