using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Enums;
using RokuTelnet.Events;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Toolbar
{
    public class ToolbarViewModel : Prism.Mvvm.BindableBase, IToolbarViewModel
    {
        private const string FILE_NAME = "ips.json";
        private const string FOLDER_LIST_NAME = "listFolder.json";
        private const string LAST_FILE_NAME = "lastIp.json";
        private const string LAST_FOLDER_NAME = "lastFolder.json";

        private IEventAggregator _eventAggregator;
        private bool _enable = true;
        private bool _connected;
        private string _selectedIp;
        private string _folder;
        private int _port = 8085;

        public ToolbarViewModel(IToolbarView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;

            IPList = new ObservableCollection<string>(LoadIpList());
            FolderList = new ObservableCollection<string>(LoadFolderList());

            AddCommand = new DelegateCommand(() =>
            {
                IPList.Add(SelectedIP);
                AddCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                UpdateFileList();
            }, () => ValidateIP(SelectedIP) && !IPList.Contains(SelectedIP));

            RemoveCommand = new DelegateCommand(() =>
            {
                IPList.Remove(SelectedIP);
                AddCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                UpdateFileList();
            }, () => ValidateIP(SelectedIP) && IPList.Contains(SelectedIP));

            ConnectCommand = new DelegateCommand(() =>
            {
                Connected = !Connected;

                UpdateLastIp();
            }, () => ValidateIP(SelectedIP));

            DeployCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<DeployEvent>().Publish(new DeployModel(SelectedIP, Folder));
            }, ()=> Connected);

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.SelectedPath = Folder;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Folder = dialog.SelectedPath;
                    UpdateLastFolder();
                    FolderList.Add(Folder);
                    UpdateFolderList();
                }
            });

            ConfigCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ShowConfigEvent>().Publish(Folder);
            });

            LaunchAppCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<SendCommandEvent>().Publish(new EventModel(EventType.Launch, EventKey.Dev));
            }, () => Connected);

            Command = new DelegateCommand<DebuggerCommandEnum?>(cmd =>
            {
                if (cmd.HasValue)
                    _eventAggregator.GetEvent<CommandEvent>().Publish(new CommandModel(_port, cmd.ToString()));
            });

            //_eventAggregator.GetEvent<LogEvent>().Subscribe(msg => Enable = msg.Message.Contains("Debugger>"), ThreadOption.UIThread);
            _eventAggregator.GetEvent<OutputChangeEvent>().Subscribe(p => _port = p);

            _eventAggregator.GetEvent<ConnectEvent>().Subscribe(ip => Connected = true, ThreadOption.UIThread);
            _eventAggregator.GetEvent<DisconnectEvent>().Subscribe(obj => Connected = false, ThreadOption.UIThread);

            LoadLastIp();
            LoadLastFolder();
        }

        public IToolbarView View { get; set; }

        public DelegateCommand<DebuggerCommandEnum?> Command { get; set; }

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; OnPropertyChanged(() => Enable); }
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand RemoveCommand { get; set; }
        public DelegateCommand ConnectCommand { get; set; }
        public ObservableCollection<string> IPList { get; set; }

        public string SelectedIP
        {
            get { return _selectedIp; }
            set
            {
                _selectedIp = value;

                AddCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnPropertyChanged(() => Connected);
                    DeployCommand.RaiseCanExecuteChanged();
                    LaunchAppCommand.RaiseCanExecuteChanged();

                    if (_connected)
                        _eventAggregator.GetEvent<ConnectEvent>().Publish(SelectedIP);
                    else
                        _eventAggregator.GetEvent<DisconnectEvent>().Publish(null);
                }
            }
        }

        public DelegateCommand OpenFolderCommand { get; set; }
        public DelegateCommand ConfigCommand { get; set; }
        public DelegateCommand DeployCommand { get; set; }
        public DelegateCommand LaunchAppCommand { get; set; }

        public string Folder
        {
            get { return _folder; }
            set { _folder = value; OnPropertyChanged(() => Folder); }
        }

        public ObservableCollection<string> FolderList { get; set; }

        private void UpdateFileList()
        {
            var content = JsonConvert.SerializeObject(IPList.ToList());

            if (File.Exists(FILE_NAME))
                File.Delete(FILE_NAME);

            using (var sw = new StreamWriter(FILE_NAME))
            {
                sw.Write(content);
            }
        }

        private List<string> LoadIpList()
        {
            if (File.Exists(FILE_NAME))
            {
                using (var sr = new StreamReader(FILE_NAME))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<string>>(content);
                }
            }

            return new List<string>();
        }

        private void LoadLastIp()
        {
            if (File.Exists(LAST_FILE_NAME))
            {
                using (var sr = new StreamReader(LAST_FILE_NAME))
                {
                    var content = sr.ReadToEnd();

                    var ip = JsonConvert.DeserializeObject<string>(content);

                    if (ValidateIP(ip))
                    {
                        SelectedIP = ip;
                        Connected = true;
                    }
                }
            }
        }

        private void UpdateLastIp()
        {
            if (File.Exists(LAST_FILE_NAME))
                File.Delete(LAST_FILE_NAME);

            if (Connected)
            {
                var content = JsonConvert.SerializeObject(SelectedIP);
                using (var sw = new StreamWriter(LAST_FILE_NAME))
                {
                    sw.Write(content);
                }
            }
        }

        private void LoadLastFolder()
        {
            if (File.Exists(LAST_FOLDER_NAME))
            {
                using (var sr = new StreamReader(LAST_FOLDER_NAME))
                {
                    var content = sr.ReadToEnd();

                    Folder = JsonConvert.DeserializeObject<string>(content);
                }
            }
        }

        private void UpdateLastFolder()
        {
            if (File.Exists(LAST_FOLDER_NAME))
                File.Delete(LAST_FOLDER_NAME);

            var content = JsonConvert.SerializeObject(Folder);
            using (var sw = new StreamWriter(LAST_FOLDER_NAME))
            {
                sw.Write(content);
            }
        }

        private void UpdateFolderList()
        {
            var content = JsonConvert.SerializeObject(FolderList.ToList());

            if (File.Exists(FOLDER_LIST_NAME))
                File.Delete(FOLDER_LIST_NAME);

            using (var sw = new StreamWriter(FOLDER_LIST_NAME))
            {
                sw.Write(content);
            }
        }

        private List<string> LoadFolderList()
        {
            if (File.Exists(FOLDER_LIST_NAME))
            {
                using (var sr = new StreamReader(FOLDER_LIST_NAME))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<string>>(content);
                }
            }

            return new List<string>();
        }

        private bool ValidateIP(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value,
                "^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        }
    }
}