using Prism.Commands;
using RokuTelnet.Enums;

namespace RokuTelnet.Views.Remote
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