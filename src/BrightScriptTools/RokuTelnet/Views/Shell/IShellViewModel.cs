using System.Collections.ObjectModel;
using Prism.Commands;

namespace RokuTelnet.Views.Shell
{
    public interface IShellViewModel
    {
        IShellView View { get; set; }
    }
}