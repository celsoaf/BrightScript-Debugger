using System.Threading.Tasks;
using BrightScript.ToolWindows.Models;

namespace BrightScript.ToolWindows.Services.Remote
{
    public interface IRemoteService
    {
        void Send(string ip, EventModel evt);
        Task SendAsync(string ip, EventModel evt);
    }
}