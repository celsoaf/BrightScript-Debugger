using System.Collections.ObjectModel;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Output
{
    public class OutputViewModel :IOutputViewModel
    {
        private const int LOGS_LENGHT = 1000;

        private readonly IEventAggregator _eventAggregator;
        
        public OutputViewModel(IOutputView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            Logs = new ObservableCollection<string>();

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg =>
            {
                if (Logs.Count >= LOGS_LENGHT)
                    Logs.RemoveAt(0);

                Logs.Add(msg);
            }, ThreadOption.UIThread);
        }

        public IOutputView View { get; set; }

        public ObservableCollection<string> Logs { get; set; }
    }
}