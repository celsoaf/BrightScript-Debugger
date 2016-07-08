using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Events;
using RokuTelnet.Events;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace RokuTelnet.Views.Screenshot
{
    public class ScreenshotViewModel : Prism.Mvvm.BindableBase, IScreenshotViewModel
    {
        private ImageSource _image;
        private bool _running;

        public ScreenshotViewModel(IScreenshotView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            eventAggregator.GetEvent<ScreenshotEvent>().Subscribe(img =>
            {
                Image = CreateBitmapSourceFromGdiBitmap(img as Bitmap);
            }, ThreadOption.UIThread);

            StartCommand = new DelegateCommand(() => eventAggregator.GetEvent<ScreenshotStartEvent>().Publish(null), () => !Running);
            StopCommand = new DelegateCommand(() => eventAggregator.GetEvent<ScreenshotStopEvent>().Publish(null), () => Running);

            eventAggregator.GetEvent<ScreenshotRunningEvent>().Subscribe(state => Running = state, ThreadOption.UIThread);
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
    }
}