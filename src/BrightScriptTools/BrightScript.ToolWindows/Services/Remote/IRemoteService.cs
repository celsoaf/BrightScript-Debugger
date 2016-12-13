using System.Threading.Tasks;
using BrightScript.ToolWindows.Models;

namespace BrightScript.ToolWindows.Services.Remote
{
    public interface IRemoteService
    {
        void Send(EventModel evt);
        Task SendAsync(EventModel evt);
        void SetArgs(string args);
    }
}