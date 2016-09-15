using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Output
{
    public class OutputViewModel : Prism.Mvvm.BindableBase, IOutputViewModel
    {
        private const int LOGS_LENGHT = 100000;

        private readonly IEventAggregator _eventAggregator;
        private string _logs;

        private string _commands;
        private int _cmdIndex = 0;
        private bool _enable = true;

        public OutputViewModel(IOutputView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            LastCommands = new ObservableCollection<string>();
            Logs = string.Empty;

            _eventAggregator.GetEvent<LogEvent>().Subscribe(msg =>
            {
                if (msg.Port == Port)
                {
                    Logs += msg.Message;

                    if (Logs.Length > LOGS_LENGHT)
                        Logs = Logs.Substring(Logs.Length - LOGS_LENGHT);
                }
            }, ThreadOption.UIThread);

            EnterCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<CommandEvent>().Publish(new CommandModel(Port, Command));
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
                    View.SetFocus();
                    View.SetCursorPosition();
                }
                View.SetFocus();
            });

            DownCommand = new DelegateCommand(() =>
            {
                if (_cmdIndex < LastCommands.Count - 1)
                {
                    _cmdIndex++;
                    Command = LastCommands[_cmdIndex];
                    View.SetFocus();
                    View.SetCursorPosition();
                }
                else
                {
                    _cmdIndex = LastCommands.Count;
                    Command = String.Empty;
                }
                View.SetFocus();
            });

            //_eventAggregator.GetEvent<DebugEvent>().Subscribe(enable => Enable = enable, ThreadOption.UIThread);
            //_eventAggregator.GetEvent<LogEvent>().Subscribe(msg => Enable = msg.Message.Contains("Debugger>"), ThreadOption.UIThread);
        }

        public IOutputView View { get; set; }

        public string Logs
        {
            get { return _logs; }
            set { _logs = value; OnPropertyChanged(() => Logs); }
        }

        public ObservableCollection<string> LastCommands { get; set; }

        public string Command
        {
            get { return _commands; }
            set
            {
                _commands = value;
                OnPropertyChanged(() => Command);
            }
        }

        public DelegateCommand EnterCommand { get; set; }
        public DelegateCommand UpCommand { get; set; }
        public DelegateCommand DownCommand { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                OnPropertyChanged(() => Enable);
                View.SetFocus();
            }
        }

        public int Port { get; set; }

        public void SetActive()
        {
            _eventAggregator.GetEvent<OutputChangeEvent>().Publish(Port);
        }
    }
}