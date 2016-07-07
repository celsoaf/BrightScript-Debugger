namespace RokuTelnet.Services.Screenshot
{
    public interface IScreenshotService
    {
        void Start(string ip);
        void Stop();
    }
}