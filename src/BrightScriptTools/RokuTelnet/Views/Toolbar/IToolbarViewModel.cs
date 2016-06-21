using Prism.Commands;
using RokuTelnet.Enums;

namespace RokuTelnet.Views.Toolbar
{
    public interface IToolbarViewModel
    {
        IToolbarView View { get; set; }

        DelegateCommand<DebuggerCommandEnum?> Command { get; set; }

        bool Enable { get; set; }
    }
}