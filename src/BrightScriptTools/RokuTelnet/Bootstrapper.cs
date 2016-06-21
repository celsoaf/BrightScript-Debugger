﻿using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Unity;
using RokuTelnet.Controllers;
using RokuTelnet.Services.Telnet;
using RokuTelnet.Views.Input;
using RokuTelnet.Views.Locals;
using RokuTelnet.Views.Output;
using RokuTelnet.Views.Shell;
using RokuTelnet.Views.StackPanel;
using RokuTelnet.Views.Toolbar;
using RokuTelnet.Views.Watch;

namespace RokuTelnet
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var shell = Container.Resolve<IShellViewModel>();


            return shell.View as DependencyObject;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            ((IShellView)Shell).Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            ConfigureServiceLocator();


            Container.RegisterType<IAppController, AppController>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IShellView, ShellView>();
            Container.RegisterType<IShellViewModel, ShellViewModel>();

            Container.RegisterType<IInputView, InputView>();
            Container.RegisterType<IInputViewModel, InputViewModel>();

            Container.RegisterType<IOutputView, OutputView>();
            Container.RegisterType<IOutputViewModel, OutputViewModel>();

            Container.RegisterType<IStackPanelView, StackPanelView>();
            Container.RegisterType<IStackPanelViewModel, StackPanelViewModel>();

            Container.RegisterType<IToolbarView, ToolbarView>();
            Container.RegisterType<IToolbarViewModel, ToolbarViewModel>();

            Container.RegisterType<IWatchView, WatchView>();
            Container.RegisterType<IWatchViewModel, WatchViewModel>();

            Container.RegisterType<ILocalsView, LocalsView>();
            Container.RegisterType<ILocalsViewModel, LocalsViewModel>();

            //Container.RegisterType<ITelenetService, TelenetService>();
            //Container.RegisterType<ITelenetService, SuperSocketService>();
            Container.RegisterType<ITelenetService, SoketService>();
            

            Container.Resolve<IAppController>().Initialize();
        }
    }
}