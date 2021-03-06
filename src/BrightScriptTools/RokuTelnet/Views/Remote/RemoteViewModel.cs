﻿using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Enums;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Remote
{
    public class RemoteViewModel : Prism.Mvvm.BindableBase, IRemoteViewModel
    {
        private bool _connected;
        private readonly IEventAggregator _eventAggregator;
        private string _input;

        public RemoteViewModel(IRemoteView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            SendCommand = new DelegateCommand<EventKey?>(cmd =>
            {
                if (cmd.HasValue)
                    _eventAggregator.GetEvent<SendCommandEvent>().Publish(new EventModel(EventType.KeyPress, cmd.Value));
            }, cmd => Connected);

            BackspaceCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<SendCommandEvent>().Publish(new EventModel(EventType.KeyPress, EventKey.Backspace));
            }, ()=> Connected);

            _eventAggregator.GetEvent<ConnectEvent>().Subscribe(ip => Connected = true, ThreadOption.UIThread);
            _eventAggregator.GetEvent<DisconnectEvent>().Subscribe(obj => Connected = false, ThreadOption.UIThread);
        }

        public IRemoteView View { get; set; }

        public DelegateCommand<EventKey?> SendCommand { get; set; }
        public DelegateCommand BackspaceCommand { get; set; }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                OnPropertyChanged(() => Connected);
                SendCommand.RaiseCanExecuteChanged();
                BackspaceCommand.RaiseCanExecuteChanged();
            }
        }

        public string Input
        {
            get { return _input; }
            set
            {
                //_input = value;

                ProcessInput(value);

                OnPropertyChanged(()=> Input);
            }
        }
        
        private void ProcessInput(string value)
        {
            Task.Factory.StartNew(() =>
            {
                value.ForEach(c =>
                {
                    _eventAggregator.GetEvent<SendCommandEvent>()
                        .Publish(new EventModel(EventType.KeyPress, EventKey.Lit_, c.ToString()));
                    Task.Delay(100).Wait();
                });
            });
        }
    }
}