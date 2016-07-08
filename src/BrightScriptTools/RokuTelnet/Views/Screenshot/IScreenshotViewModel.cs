using System.Windows.Media;
using Prism.Commands;

namespace RokuTelnet.Views.Screenshot
{
    public interface IScreenshotViewModel
    {
        IScreenshotView View { get; set; }

        ImageSource Image { get; set; }

        DelegateCommand StartCommand { get; set; }
        DelegateCommand StopCommand { get; set; }

        bool Running { get; set; }
    }
}