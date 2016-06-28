using System.Collections.ObjectModel;
using Prism.Commands;
using RokuTelnet.Views.Output;

namespace RokuTelnet.Views.Cygwin
{
    public interface ICygwinViewModel
    {
        ICygwinView View { get; set; }

        string Output { get; set; }

        ObservableCollection<string> LastCommands { get; set; }

        string Command { get; set; }
        DelegateCommand EnterCommand { get; set; }
        DelegateCommand UpCommand { get; set; }
        DelegateCommand DownCommand { get; set; }

        bool Enable { get; set; }
    }
}