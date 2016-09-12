using System.Collections.ObjectModel;
using Prism.Commands;

namespace RokuTelnet.Views.Output
{
    public interface IOutputViewModel
    {
        IOutputView View { get; set; }

        string Logs { get; set; }

        ObservableCollection<string> LastCommands { get; set; }

        string Command { get; set; }
        DelegateCommand EnterCommand { get; set; }
        DelegateCommand UpCommand { get; set; }
        DelegateCommand DownCommand { get; set; }

        bool Enable { get; set; }

        int Port { get; set; }

        void SetActive();
    }
}