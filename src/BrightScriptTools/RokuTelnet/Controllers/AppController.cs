using System;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using RokuTelnet.Events;
using RokuTelnet.Services.Parser;
using RokuTelnet.Services.Telnet;
using RokuTelnet.Views.Input;
using RokuTelnet.Views.Locals;
using RokuTelnet.Views.Output;
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

        private IRegionManager _regionManager;

        public AppController(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IParserService parserService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _parserService = parserService;
            _container = container;
        }

        public async void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OUTPUT, () => _container.Resolve<IOutputViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.INPUT, () => _container.Resolve<IInputViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.TOOLBAR, () => _container.Resolve<IToolbarViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.STACK_PANEL, () => _container.Resolve<IStackPanelViewModel>().View);
            //_regionManager.RegisterViewWithRegion(RegionNames.WATCH, () => _container.Resolve<IWatchViewModel>().View);
            _regionManager.RegisterViewWithRegion(RegionNames.LOCALS, () => _container.Resolve<ILocalsViewModel>().View);

            Task.Factory.StartNew(() =>
            {
                Task.Delay(1000).Wait();
                _parserService.Start();
                Connect("192.168.1.108", 8085).Wait();
            }, TaskCreationOptions.LongRunning);
            

            _eventAggregator.GetEvent<CommandEvent>().Subscribe(cmd =>
            {
               _telenetService.Send(cmd);
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

                LogFormat("Connected {0}-{1}", ip, port);
            }
            else
                LogFormat("Not connected {0}-{1}", ip, port);
        }

        private void Log(string msg)
        {
            _eventAggregator.GetEvent<LogEvent>().Publish(msg);
        }

        private void LogFormat(string msg, params object[] args)
        {
            _eventAggregator.GetEvent<LogEvent>().Publish(string.Format(msg, args));
        }
    }
}