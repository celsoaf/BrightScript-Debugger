using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using RokuTelnet.Enums;
using RokuTelnet.Events;
using RokuTelnet.Models;
using RokuTelnet.Services.Deploy;
using RokuTelnet.Services.Parser;
using RokuTelnet.Services.Remote;
using RokuTelnet.Services.Screenshot;
using RokuTelnet.Services.Telnet;
using RokuTelnet.Views.Config;
using RokuTelnet.Views.Console;
using RokuTelnet.Views.Cygwin;
using RokuTelnet.Views.Locals;
using RokuTelnet.Views.Output;
using RokuTelnet.Views.Remote;
using RokuTelnet.Views.Screenshot;
using RokuTelnet.Views.Shell;
using RokuTelnet.Views.StackPanel;
using RokuTelnet.Views.Toolbar;
using RokuTelnet.Views.Watch;

namespace RokuTelnet.Controllers
{
    public class AppController : IAppController
    {
        private const string OPTIONS_FILE = "deploy.json";

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRemoteService _remoteService;
        private readonly IRegionManager _regionManager;
        private readonly IDeployService _deployService;
        private readonly IScreenshotService _screenshotService;
        private IShellViewModel _shell;

        private IDictionary<int, ITelnetService> _telnetTasks = new Dictionary<int, ITelnetService>();
        private IDictionary<int, IParserService> _parserTasks = new Dictionary<int, IParserService>();

        private readonly Dictionary<DebuggerCommandEnum, string> _injectStrings;
        private DebuggerCommandEnum? _lasCommand;
        private volatile bool _connected;
        private string _ip;
        private volatile bool _debug = true;
        private volatile bool _screenshotRunning;
        private IToolbarViewModel _toolbarViewModel;
        private int _selectedPort;

        public AppController(
            IUnityContainer container,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            IRemoteService remoteService,
            IDeployService deployService,
            IScreenshotService screenshotService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _remoteService = remoteService;
            _deployService = deployService;
            _screenshotService = screenshotService;
            _container = container;

            _injectStrings = new Dictionary<DebuggerCommandEnum, string>();
            _injectStrings.Add(DebuggerCommandEnum.bt, "Backtrace: ");
            _injectStrings.Add(DebuggerCommandEnum.var, "Local Variables: ");
            _injectStrings.Add(DebuggerCommandEnum.list, "Current Function: ");
        }

        public async void Initialize()
        {
            RegisterTelnet(RegionNames.OUTPUT_MAIN, 8085);
            RegisterTelnet(RegionNames.OUTPUT_SCENE_GRAPH, 8089);
            RegisterTelnet(RegionNames.OUTPUT_TASK_1, 8090);
            RegisterTelnet(RegionNames.OUTPUT_TASK_2, 8091);
            RegisterTelnet(RegionNames.OUTPUT_TASK_3, 8092);
            RegisterTelnet(RegionNames.OUTPUT_TASK_REST, 8093);
            //RegisterTelnet(RegionNames.OUTPUT_SPECIAL, 8080);
            _regionManager.RegisterViewWithRegion(RegionNames.OUTPUT_MAIN, () =>
            {
                var vm = _container.Resolve<IOutputViewModel>();
                vm.Port = 8085;
                return vm.View;
            });
            _regionManager.RegisterViewWithRegion(RegionNames.OUTPUT_SCENE_GRAPH, () =>
            {
                var vm = _container.Resolve<IOutputViewModel>();
                vm.Port = 8089;
                return vm.View;
            });
            _regionManager.RegisterViewWithRegion(RegionNames.TOOLBAR, () =>
            {
                _toolbarViewModel = _container.Resolve<IToolbarViewModel>();
                return _toolbarViewModel.View;
            });
            _regionManager.RegisterViewWithRegion(RegionNames.STACK_PANEL, () => _container.Resolve<IStackPanelViewModel>().View);
            //_regionManager.RegisterViewWithRegion(RegionNames.WATCH, () => _container.Resolve<IWatchViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.LOCALS, () => _container.Resolve<ILocalsViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.CONSOLE, () => _container.Resolve<IConsoleViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.REMOTE, () => _container.Resolve<IRemoteViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.CYGWIN, () => _container.Resolve<ICygwinViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.SCREENSHOT, () => _container.Resolve<IScreenshotViewModel>().View);

            _eventAggregator.GetEvent<CommandEvent>().Subscribe(SendCommand);

            _eventAggregator.GetEvent<ConnectEvent>().Subscribe(ip =>
            {
                _ip = ip;
                Task.Delay(1000).Wait();
                Connect(ip, 8085).Wait();
                Connect(ip, 8089).Wait();
                Connect(ip, 8090).Wait();
                Connect(ip, 8091).Wait();
                Connect(ip, 8092).Wait();
                Connect(ip, 8093).Wait();
                Connect(ip, 8080).Wait();

                var args = JsonConvert.SerializeObject(new { ip = ip });
                _remoteService.SetArgs(args);
            }, ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<DisconnectEvent>().Subscribe(obj =>
            {
                _telnetTasks.Values.ForEach(t=> t.Disconnect());
                _telnetTasks.Clear();
                _parserTasks.Values.ForEach(p=> p.Stop());
                _parserTasks.Clear();
                _screenshotService.Stop();
                _connected = false;
            }, ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<SendCommandEvent>().Subscribe(cmd =>
            {
                _remoteService.SendAsync(cmd);
            });

            //_eventAggregator.GetEvent<DebugEvent>().Subscribe(enabled => _debug = enabled);

            _eventAggregator.GetEvent<DeployEvent>().Subscribe(model => Deploy(model.Ip, model.Folder), ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<ShowConfigEvent>().Subscribe(f => ShowConfig(f));

            _eventAggregator.GetEvent<BusyShowEvent>().Subscribe(m => _screenshotService.Stop());
            _eventAggregator.GetEvent<BusyHideEvent>().Subscribe(obj =>
            {
                if (_screenshotRunning)
                    _screenshotService.Start(_ip);
            });

            _eventAggregator.GetEvent<ScreenshotStartEvent>().Subscribe(obj =>
            {
                _screenshotRunning = true;
                _screenshotService.Start(_ip);
            });

            _eventAggregator.GetEvent<ScreenshotStopEvent>().Subscribe(obj =>
            {
                _screenshotRunning = false;
                _screenshotService.Stop();
            });

            _eventAggregator.GetEvent<OutputChangeEvent>().Subscribe(p => _selectedPort = p);
            _eventAggregator.GetEvent<OutputChangeEvent>().Publish(8085);

            RegisterCommands();
        }

        private void RegisterTelnet(string region, int port)
        {
            _regionManager.RegisterViewWithRegion(region, () =>
            {
                var vm = _container.Resolve<IOutputViewModel>();
                vm.Port = port;
                return vm.View;
            });
        }

        private void Deploy(string ip, string folder)
        {
            _telnetTasks.ForEach(kv =>
            {
                kv.Value.Send(DebuggerCommandEnum.exit.ToString());
            });

            var optionsFile = Path.Combine(folder, OPTIONS_FILE);

            if (File.Exists(optionsFile))
                _deployService.Deploy(ip, folder, optionsFile);
            else
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (ShowConfig(folder) == true)
                        Task.Factory.StartNew(() => _deployService.Deploy(ip, folder, optionsFile));
                }));
            }
        }

        private bool? ShowConfig(string folder)
        {
            var optionsFile = Path.Combine(folder, OPTIONS_FILE);

            var vm = _container.Resolve<IConfigViewModel>();
            vm.Load(optionsFile);
            return vm.View.ShowDialog() == true;
        }

        private void SendCommand(CommandModel cmd)
        {
            var port = GetCurrentViewPort();

            _telnetTasks[cmd.Port].Send(cmd.Command);

            Log(port, cmd.Command + Environment.NewLine);

            DebuggerCommandEnum c;
            if (Enum.TryParse(cmd.Command, out c))
                _lasCommand = c;
            else
                _lasCommand = null;
        }

        private int GetCurrentViewPort()
        {
            return _selectedPort;
        }

        private void RegisterCommands()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                RegisterDebuggerCommand(GlobalCommands.DebuggerStepIn, DebuggerCommandEnum.s);
                RegisterDebuggerCommand(GlobalCommands.DebuggerStepOver, DebuggerCommandEnum.over);
                RegisterDebuggerCommand(GlobalCommands.DebuggerStepOut, DebuggerCommandEnum.@out);
                RegisterDebuggerCommand(GlobalCommands.DebuggerContinue, DebuggerCommandEnum.c);

                RegisterDebuggerCommand(GlobalCommands.DebuggerStop, DebuggerCommandEnum.exit);
                RegisterDebuggerCommand(GlobalCommands.DebuggerDown, DebuggerCommandEnum.d);
                RegisterDebuggerCommand(GlobalCommands.DebuggerUp, DebuggerCommandEnum.u);
                RegisterDebuggerCommand(GlobalCommands.DebuggerBacktrace, DebuggerCommandEnum.bt);
                //RegisterDebuggerCommand(GlobalCommands.DebuggerVariables, DebuggerCommandEnum.var);
                RegisterDebuggerCommand(GlobalCommands.DebuggerFunction, DebuggerCommandEnum.list);

                App.Current.MainWindow.CommandBindings.Add(new CommandBinding(GlobalCommands.Deploy, (s, e) =>
                {
                    if (_connected && _toolbarViewModel != null)
                    {
                        Task.Factory.StartNew(() => Deploy(_toolbarViewModel.SelectedIP, _toolbarViewModel.Folder));
                    }
                }));
            }));
        }

        private void RegisterDebuggerCommand(RoutedUICommand command, DebuggerCommandEnum cmd)
        {
            App.Current.MainWindow.CommandBindings.Add(new CommandBinding(command, (s, e) =>
            {
                if (_debug)
                {
                    SendCommand(new CommandModel(GetCurrentViewPort(), cmd.ToString()));
                }
            }));
        }

        private async Task Connect(string ip, int port)
        {
            var telnet = _container.Resolve<ITelnetService>();
            telnet.Log += l => Log(telnet.Port, l);
            telnet.Close += () => LogFormat(port, "Connection closed");
            _connected = await telnet.Connect(ip, port);

            if (_connected)
            {
                _telnetTasks[port] = telnet;
                var parser = _container.Resolve<IParserService>();
                parser.Start(port);
                _parserTasks[port] = parser;

                App.Current.Dispatcher.BeginInvoke(
                    new Action(() => App.Current.Exit += (s, e) => telnet.Disconnect()));

                LogFormat(port, "Connected {0}:{1}" + Environment.NewLine, ip, port);
            }
            else
                LogFormat(port, "Not connected {0}:{1}" + Environment.NewLine, ip, port);
        }

        private void Log(int port, string msg)
        {
            if (_lasCommand.HasValue)
            {
                if (_injectStrings.ContainsKey(_lasCommand.Value))
                    msg = _injectStrings[_lasCommand.Value] + Environment.NewLine + msg;

                _lasCommand = null;
            }

            //_debug = msg.Contains("Debugger>");

            _eventAggregator.GetEvent<LogEvent>().Publish(new LogModel(port, msg));
        }

        private void LogFormat(int port, string msg, params object[] args)
        {
            _eventAggregator.GetEvent<LogEvent>().Publish(new LogModel(port, string.Format(msg, args)));
        }
    }
}