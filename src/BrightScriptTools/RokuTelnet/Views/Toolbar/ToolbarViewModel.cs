using Prism.Commands;
using Prism.Events;
using RokuTelnet.Enums;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Toolbar
{
    public class ToolbarViewModel : Prism.Mvvm.BindableBase, IToolbarViewModel
    {
        private IEventAggregator _eventAggregator;
        private bool _enable;

        public ToolbarViewModel(IToolbarView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            Command = new DelegateCommand<DebuggerCommandEnum?>(cmd =>
            {
                if (cmd.HasValue)
                    _eventAggregator.GetEvent<CommandEvent>().Publish(cmd.ToString());
            });

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg => Enable = msg.Contains("Debugger>"));
        }

        public IToolbarView View { get; set; }

        public DelegateCommand<DebuggerCommandEnum?> Command { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; OnPropertyChanged(() => Enable); }
        }
    }
}