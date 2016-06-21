using System.Collections.ObjectModel;
using Prism.Commands;

namespace RokuTelnet.Views.Input
{
    public interface IInputViewModel
    {
        IInputView View { get; set; }

        ObservableCollection<string> LastCommands { get; set; }

        string Command { get; set; }
        DelegateCommand EnterCommand { get; set; }
        DelegateCommand UpCommand { get; set; }
        DelegateCommand DownCommand { get; set; }

        bool Enable { get; set; }
    }
}