using System.Collections.ObjectModel;
using RokuTelnet.Models;

namespace RokuTelnet.Views.StackPanel
{
    public interface IStackPanelViewModel
    {
        IStackPanelView View { get; set; }

        ObservableCollection<BacktraceModel> List { get; set; }
    }
}