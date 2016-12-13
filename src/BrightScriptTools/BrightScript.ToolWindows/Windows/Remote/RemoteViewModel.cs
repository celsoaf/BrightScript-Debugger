using System.Threading.Tasks;
using System.Linq;
using BrightScript.ToolWindows.Enums;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;

namespace BrightScript.ToolWindows.Windows.Remote
{
    public class RemoteViewModel : Prism.Mvvm.BindableBase, IRemoteViewModel
    {
        private bool _connected;
        private string _input;

        public RemoteViewModel(IRemoteView view)
        {
            View = view;
            View.DataContext = this;

            Connected = true;

            SendCommand = new DelegateCommand<EventKey?>(cmd =>
            {

            }, cmd => Connected);

            BackspaceCommand = new DelegateCommand(() =>
            {

            }, () => Connected);
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

                    Task.Delay(100).Wait();
                });
            });
        }
    }
}