using System;
using System.Collections.ObjectModel;
using System.Windows.Interop;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Input
{
    public class InputViewModel : Prism.Mvvm.BindableBase,  IInputViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _commands;
        private int _cmdIndex = 0;
        private bool _enable;

        public InputViewModel(IInputView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            LastCommands = new ObservableCollection<string>();

            _eventAggregator = eventAggregator;

            EnterCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<CommandEvent>().Publish(Command);
                LastCommands.Add(Command);
                if (LastCommands.Count > 100)
                    LastCommands.RemoveAt(0);
                _cmdIndex = LastCommands.Count;
                Command = string.Empty;
            });

            UpCommand = new DelegateCommand(() =>
            {
                if (_cmdIndex > 0)
                {
                    _cmdIndex--;
                    Command = LastCommands[_cmdIndex];
                }
            });

            DownCommand = new DelegateCommand(() =>
            {
                if (_cmdIndex < LastCommands.Count - 1)
                {
                    _cmdIndex++;
                    Command = LastCommands[_cmdIndex];
                }
                else
                {
                    _cmdIndex = LastCommands.Count;
                    Command = String.Empty;
                }
            });

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg => Enable = msg.Contains("Debugger>"));
        }

        public IInputView View { get; set; }

        public ObservableCollection<string> LastCommands { get; set; }

        public string Command
        {
            get { return _commands; }
            set { _commands = value; OnPropertyChanged(() => Command); }
        }

        public DelegateCommand EnterCommand { get; set; }
        public DelegateCommand UpCommand { get; set; }
        public DelegateCommand DownCommand { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; OnPropertyChanged(()=> Enable); }
        }
    }
}