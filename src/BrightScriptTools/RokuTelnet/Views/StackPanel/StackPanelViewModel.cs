using System.Collections.ObjectModel;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.StackPanel
{
    public class StackPanelViewModel : IStackPanelViewModel
    {
        private IEventAggregator _eventAggregator;

        public StackPanelViewModel(IStackPanelView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            List = new ObservableCollection<BacktraceModel>();

            _eventAggregator.GetEvent<BacktraceEvent>().Subscribe(trace =>
            {
                List.Clear();
                List.AddRange(trace);
            }, ThreadOption.UIThread);
        }

        public IStackPanelView View { get; set; }

        public ObservableCollection<BacktraceModel> List { get; set; }
    }
}