using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Shell
{
    public class ShellViewModel : IShellViewModel
    {
        public ShellViewModel(IShellView view)
        {
            View = view;
            View.DataContext = this;
        }

        public IShellView View { get; set; }
    }
}