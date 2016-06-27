using System.Threading.Tasks;
using RokuTelnet.Models;

namespace RokuTelnet.Services.Remote
{
    public interface IRemoteService
    {
        void Send(EventModel evt);
        Task SendAsync(EventModel evt);
        void SetArgs(string args);
    }
}