using System.Collections.ObjectModel;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Locals
{
    public class LocalsViewModel : ILocalsViewModel
    {
        private IEventAggregator _eventAggregator;

        public LocalsViewModel(ILocalsView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            List = new ObservableCollection<VariableModel>();

            _eventAggregator.GetEvent<VariablesEvent>().Subscribe(vars =>
            {
                List.Clear();
                List.AddRange(vars);
            }, ThreadOption.UIThread);
        }

        public ILocalsView View { get; set; }

        public ObservableCollection<VariableModel> List { get; set; }
    }
}