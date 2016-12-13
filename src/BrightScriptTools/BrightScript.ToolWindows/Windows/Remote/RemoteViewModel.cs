using System.Threading.Tasks;
using System.Linq;
using BrightScript.ToolWindows.Enums;
using BrightScript.ToolWindows.Models;
using BrightScript.ToolWindows.Services.Remote;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;

namespace BrightScript.ToolWindows.Windows.Remote
{
    public class RemoteViewModel : Prism.Mvvm.BindableBase, IRemoteViewModel
    {
        private readonly IRemoteService _remoteService;
        private bool _connected;
        private string _input;

        public RemoteViewModel(IRemoteView view, IRemoteService remoteService)
        {
            _remoteService = remoteService;
            View = view;
            View.DataContext = this;

            SendCommand = new DelegateCommand<EventKey?>(cmd =>
            {
                if(cmd.HasValue)
                    _remoteService.SendAsync(new EventModel(EventType.KeyPress, cmd.Value));
            }, cmd => Connected);

            BackspaceCommand = new DelegateCommand(() =>
            {
                _remoteService.SendAsync(new EventModel(EventType.KeyPress, EventKey.Backspace));
            }, () => Connected);

            Connected = true;
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

                OnPropertyChanged(() => Input);
            }
        }



        private void ProcessInput(string value)
        {
            Task.Factory.StartNew(() =>
            {
                value.ForEach(c =>
                {
                    _remoteService.Send(new EventModel(EventType.KeyPress, EventKey.Lit_, c.ToString()));
                    Task.Delay(100).Wait();
                });
            });
        }
    }
}