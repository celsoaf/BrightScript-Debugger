using System.Linq;
using BrightScript.ToolWindows.Enums;
using BrightScript.ToolWindows.Models;
using BrightScript.ToolWindows.Services.Remote;
using EnvDTE;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.VisualStudio.Shell;
using Prism.Commands;
using Task = System.Threading.Tasks.Task;

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
                    _remoteService.SendAsync(GetIp(), new EventModel(EventType.KeyPress, cmd.Value));
            }, cmd => Connected);

            BackspaceCommand = new DelegateCommand(() =>
            {
                _remoteService.SendAsync(GetIp(), new EventModel(EventType.KeyPress, EventKey.Backspace));
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
                    _remoteService.Send(GetIp(), new EventModel(EventType.KeyPress, EventKey.Lit_, c.ToString()));
                    Task.Delay(100).Wait();
                });
            });
        }

        private string GetIp()
        {
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));

            Projects projects = dte.Solution.Projects;
            if (projects.Count > 0)
            {
                Project project = projects.Item(1);
                if (project != null && project.Properties != null)
                {
                    foreach (Property property in project.Properties)
                    {
                        if (property.Name == "BoxIP" && property.Value != null)
                            return property.Value.ToString();
                    }
                }
            }

            return null;
        }
    }
}