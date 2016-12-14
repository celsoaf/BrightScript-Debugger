using BrightScript.ToolWindows.Enums;
using Prism.Commands;

namespace BrightScript.ToolWindows.Windows.Remote
{
    public interface IRemoteViewModel
    {
        IRemoteView View { get; set; }

        DelegateCommand<EventKey?> SendCommand { get; set; }

        DelegateCommand BackspaceCommand { get; set; }

        bool Connected { get; set; }

        string Input { get; set; }
    }
}