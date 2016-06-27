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

            _eventAggregator.GetEvent<ConnectEvent>().Subscribe(ip => Connected = true);
            _eventAggregator.GetEvent<DisconnectEvent>().Subscribe(obj => Connected = false);
        }

        public IRemoteView View { get; set; }

        public DelegateCommand<EventKey?> SendCommand { get; set; }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                OnPropertyChanged(() => Connected);
                SendCommand.RaiseCanExecuteChanged();
            }
        }
    }
}