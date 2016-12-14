using System.Windows.Media;
using Prism.Commands;

namespace BrightScript.ToolWindows.Windows.Screenshot
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