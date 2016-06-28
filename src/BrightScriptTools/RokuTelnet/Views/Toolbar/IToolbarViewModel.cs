using System.Collections.ObjectModel;
using Prism.Commands;
using RokuTelnet.Enums;

namespace RokuTelnet.Views.Toolbar
{
    public interface IToolbarViewModel
    {
        IToolbarView View { get; set; }

        DelegateCommand<DebuggerCommandEnum?> Command { get; set; }

        bool Enable { get; set; }

        DelegateCommand AddCommand { get; set; }
        DelegateCommand RemoveCommand { get; set; }
        DelegateCommand ConnectCommand { get; set; }

        ObservableCollection<string> IPList { get; set; }
        string SelectedIP { get; set; }
        bool Connected { get; set; }

        DelegateCommand OpenFolderCommand { get; set; }
        DelegateCommand DeployCommand { get; set; }

        string Folder { get; set; }
    }
}