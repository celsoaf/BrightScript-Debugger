using System;
using System.Drawing;

namespace BrightScript.ToolWindows.Services.Screenshot
{
    public interface IScreenshotService
    {
        void Start(string ip, string user, string pass);
        void Stop();

        event Action<Image> OnImageArrived;
    }
}