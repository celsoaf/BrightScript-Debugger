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
    }
}