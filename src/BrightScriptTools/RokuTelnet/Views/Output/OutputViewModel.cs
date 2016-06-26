using System.Collections.ObjectModel;
using System.Linq;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Output
{
    public class OutputViewModel : Prism.Mvvm.BindableBase, IOutputViewModel
    {
        private const int LOGS_LENGHT = 10000;

        private readonly IEventAggregator _eventAggregator;
        private string _logs;

        public OutputViewModel(IOutputView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            Logs = string.Empty;

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg =>
            {
                Logs += msg;

                if (Logs.Length > LOGS_LENGHT)
                    Logs = Logs.Substring(Logs.Length - LOGS_LENGHT);
            }, ThreadOption.UIThread);
        }

        public IOutputView View { get; set; }

        public string Logs
        {
            get { return _logs; }
            set { _logs = value; OnPropertyChanged(() => Logs); }
        }
    }
}