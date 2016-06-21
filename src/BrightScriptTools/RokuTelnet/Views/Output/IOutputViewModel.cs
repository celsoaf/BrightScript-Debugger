using System.Collections.ObjectModel;

namespace RokuTelnet.Views.Output
{
    public interface IOutputViewModel
    {
        IOutputView View { get; set; }

        ObservableCollection<string> Logs { get; set; }
    }
}