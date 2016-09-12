using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Shell
{
    public class ShellViewModel : Prism.Mvvm.BindableBase, IShellViewModel
    {
        private bool _isBusy;
        private BusyModel _busyModel;
        private int _selectedIndex;

        public ShellViewModel(IShellView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            eventAggregator.GetEvent<BusyShowEvent>().Subscribe(m =>
            {
                BusyModel = m;
                IsBusy = true;
            }, ThreadOption.UIThread);

            eventAggregator.GetEvent<BusyHideEvent>().Subscribe(obj =>
            {
                BusyModel = null;
                IsBusy = false;
            });
        }

        public IShellView View { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(() => IsBusy); }
        }

        public BusyModel BusyModel
        {
            get { return _busyModel; }
            set { _busyModel = value; OnPropertyChanged(() => BusyModel); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged(()=> SelectedIndex); }
        }
    }
}