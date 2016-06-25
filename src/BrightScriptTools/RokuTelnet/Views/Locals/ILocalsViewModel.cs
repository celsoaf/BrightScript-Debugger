using System.Collections.ObjectModel;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Locals
{
    public interface ILocalsViewModel
    {
        ILocalsView View { get; set; }

        ObservableCollection<VariableModel> List { get; set; }
    }
}