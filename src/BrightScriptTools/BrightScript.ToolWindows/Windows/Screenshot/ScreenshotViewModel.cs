using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BrightScript.ToolWindows.Services.Screenshot;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Prism.Commands;

namespace BrightScript.ToolWindows.Windows.Screenshot
{
    public class ScreenshotViewModel : Prism.Mvvm.BindableBase, IScreenshotViewModel
    {
        private readonly IScreenshotService _screenshotService;
        private bool _running;
        private ImageSource _image;

        public ScreenshotViewModel(IScreenshotView view, IScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
            View = view;
            View.DataContext = this;

            StartCommand = new DelegateCommand(() =>
            {
                var config = GetConfig();
                if (config != null)
                {
                    _screenshotService.Start(config.IP, config.User, config.Pass);
                    Running = true;
                }
            }, () => !Running);

            StopCommand = new DelegateCommand(() =>
            {
                _screenshotService.Stop();
                Running = false;
            }, () => Running);

            _screenshotService.OnImageArrived += img =>
            {
                ((UserControl)this.View).Dispatcher.BeginInvoke(new Action(() =>
                {
                    Image = CreateBitmapSourceFromGdiBitmap(img as Bitmap);
                }));
            };
        }

        public IScreenshotView View { get; set; }

        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(() => Image); }
        }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        public bool Running
        {
            get { return _running; }
            set
            {
                _running = value;
                OnPropertyChanged(() => Running);
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        class Config
        {
            public string IP { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
        }

        private Config GetConfig()
        {
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));

            Projects projects = dte.Solution.Projects;
            if (projects.Count > 0)
            {
                var res = new Config();
                Project project = projects.Item(1);
                if (project != null && project.Properties != null)
                {
                    foreach (Property property in project.Properties)
                    {
                        if (property.Name == "BoxIP" && property.Value != null)
                            res.IP = property.Value.ToString();

                        if (property.Name == "UserName" && property.Value != null)
                            res.User = property.Value.ToString();

                        if (property.Name == "Password" && property.Value != null)
                            res.Pass = property.Value.ToString();
                    }
                }

                return res;
            }
        
            return null;
        }
    }
}