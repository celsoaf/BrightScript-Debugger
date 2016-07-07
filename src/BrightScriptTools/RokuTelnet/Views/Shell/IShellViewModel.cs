using System.Collections.ObjectModel;
using Prism.Commands;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Shell
{
    public interface IShellViewModel
    {
        IShellView View { get; set; }

        bool IsBusy { get; set; }
        BusyModel BusyModel { get; set; }
    }
}