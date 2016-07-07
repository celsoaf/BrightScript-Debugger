using System.Windows.Media;

namespace RokuTelnet.Views.Screenshot
{
    public interface IScreenshotViewModel
    {
        IScreenshotView View { get; set; }

        ImageSource Image { get; set; }
    }
}