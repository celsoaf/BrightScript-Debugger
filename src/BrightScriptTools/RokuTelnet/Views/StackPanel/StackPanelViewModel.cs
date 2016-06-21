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

            List = new ObservableCollection<StackModel>();

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg =>
            {
                if (msg.StartsWith("#"))
                    ParseMessage(msg);
            });
        }

        private void ParseMessage(string msg)
        {
            //throw new System.NotImplementedException();
        }

        public IStackPanelView View { get; set; }

        public ObservableCollection<StackModel> List { get; set; }
    }
}