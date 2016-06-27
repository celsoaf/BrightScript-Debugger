using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Regions;
using RokuTelnet.Enums;
using RokuTelnet.Events;
using RokuTelnet.Services.Parser;
using RokuTelnet.Services.Remote;
using RokuTelnet.Services.Telnet;
using RokuTelnet.Views.Console;
using RokuTelnet.Views.Input;
using RokuTelnet.Views.Locals;
using RokuTelnet.Views.Output;
using RokuTelnet.Views.Remote;
using RokuTelnet.Views.StackPanel;
using RokuTelnet.Views.Toolbar;
using RokuTelnet.Views.Watch;

namespace RokuTelnet.Controllers
{
    public class AppController : IAppController
    {
        private IUnityContainer _container;
        private IEventAggregator _eventAggregator;
        private ITelenetService _telenetService;
        private IParserService _parserService;
        private IRemoteService _remoteService;

        private IRegionManager _regionManager;

        private Dictionary<DebuggerCommandEnum, string> _injectStrings;
        private DebuggerCommandEnum? _lasCommand;

        public AppController(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IParserService parserService, IRemoteService remoteService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _parserService = parserService;
            _remoteService = remoteService;
            _container = container;

            _injectStrings=new Dictionary<DebuggerCommandEnum, string>();
            _injectStrings.Add(DebuggerCommandEnum.bt, "Backtrace: ");
            _injectStrings.Add(DebuggerCommandEnum.var, "Local Variables: ");
            _injectStrings.Add(DebuggerCommandEnum.list, "Current Function: ");
        }

        public async void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OUTPUT, () => _container.Resolve<IOutputViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.INPUT, () => _container.Resolve<IInputViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.TOOLBAR, () => _container.Resolve<IToolbarViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.STACK_PANEL, () => _container.Resolve<IStackPanelViewModel>().View);
            //_regionManager.RegisterViewWithRegion(RegionNames.WATCH, () => _container.Resolve<IWatchViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.LOCALS, () => _container.Resolve<ILocalsViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.CONSOLE, () => _container.Resolve<IConsoleViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.REMOTE, () => _container.Resolve<IRemoteViewModel>().View);

            _eventAggregator.GetEvent<CommandEvent>().Subscribe(cmd =>
            {
                _telenetService.Send(cmd);

                Log(cmd + Environment.NewLine);

                DebuggerCommandEnum c;
                if (Enum.TryParse(cmd, out c))
                    _lasCommand = c;
                else
                    _lasCommand = null;
            });

            _eventAggregator.GetEvent<ConnectEvent>().Subscribe(ip =>
            {
                Task.Delay(1000).Wait();
                _parserService.Start();
                Connect(ip, 8085).Wait();

                var args = JsonConvert.SerializeObject(new { ip = ip });
                _remoteService.SetArgs(args);
            }, ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<DisconnectEvent>().Subscribe(obj =>
            {
                _telenetService.Disconnect();
                _parserService.Stop();
            }, ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<SendCommandEvent>().Subscribe(cmd =>
            {
                _remoteService.SendAsync(cmd);
            });
        }

        private async Task Connect(string ip, int port)
        {
            _telenetService = _container.Resolve<ITelenetService>();
            _telenetService.Log += l => Log(l);
            _telenetService.Close += () => LogFormat("Connection closed");
            var connected = await _telenetService.Connect(ip, port);

            if (connected)
            {
                App.Current.Dispatcher.BeginInvoke(
                    new Action(() => App.Current.Exit += (s, e) => _telenetService.Disconnect()));

                LogFormat("Connected {0}:{1}" + Environment.NewLine, ip, port);
            }
            else
                LogFormat("Not connected {0}:{1}" + Environment.NewLine, ip, port);
        }

        private void Log(string msg)
        {
            if (_lasCommand.HasValue)
            {
                if (_injectStrings.ContainsKey(_lasCommand.Value))
                    msg = _injectStrings[_lasCommand.Value] + Environment.NewLine + msg;

                _lasCommand = null;
            }

            _eventAggregator.GetEvent<LogEvent>().Publish(msg);
        }

        private void LogFormat(string msg, params object[] args)
        {
            _eventAggregator.GetEvent<LogEvent>().Publish(string.Format(msg, args));
        }
    }
}