using System.Collections.ObjectModel;
using Prism.Commands;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Config
{
    public interface IConfigViewModel
    {
        IConfigView View { get; set; }

        ConfigModel Model { get; set; }

        void Load(string filePath);
        void Save(string filePath);

        DelegateCommand SaveCommand { get; set; }
        DelegateCommand CancelCommand { get; set; }

        ObservableCollection<string> Includes { get; set; }
        ObservableCollection<string> Excludes { get; set; }
        ObservableCollection<ConfigKeyModel> ExtraConfigs { get; set; }
        ObservableCollection<ConfigKeyModel> Replaces { get; set; }
    }
}