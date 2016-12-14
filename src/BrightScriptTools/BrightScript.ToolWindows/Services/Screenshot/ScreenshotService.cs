using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BrightScript.ToolWindows.Services.Screenshot.Utils;
using Newtonsoft.Json;

namespace BrightScript.ToolWindows.Services.Screenshot
{
    public class ScreenshotService : IScreenshotService
    {
        private const string URL = "http://{0}//plugin_inspect";

        private const int SLEEP_TIME = 1000;

        private volatile bool _running = false;

        public void Start(string ip, string user, string pass)
        {
            if (!_running)
            {
                _running = true;

                Task.Factory.StartNew(() => Run(ip, user, pass), TaskCreationOptions.LongRunning);
            }
        }

        private void Run(string ip, string user, string pass)
        {
            while (_running)
            {
                try
                {
                    var url = GetImageUrl(ip, user, pass);

                    if (!string.IsNullOrEmpty(url))
                    {
                        var image = GetImage(url, user, pass);

                        if (image != null)
                            OnImageArrived?.Invoke(image);
                    }
                    else
                    {
                        OnImageArrived.Invoke(null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    Task.Delay(SLEEP_TIME).Wait();
                }
            }
        }

        private Image GetImage(string url, string user, string pass)
        {
            var req = new DigestHttpWebRequest(user, pass);

            Uri uri = new Uri(url);

            using (HttpWebResponse webResponse = req.GetResponse(uri))
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                return Image.FromStream(responseStream);
            }
        }

        private static string GetImageUrl(string ip, string user, string pass)
        {
            var req = new DigestHttpWebRequest(user, pass);

            req.Method = WebRequestMethods.Http.Post;

            var formData = new MultipartFormData();
            formData.AddFile("archive", "", "application/octet-stream");
            formData.Add("passwd", "");
            formData.Add("mysubmit", "Screenshot");
            req.PostData = formData.GetMultipartFormData();
            req.ContentType = formData.ContentType;

            Uri uri = new Uri(string.Format(URL, ip));

            using (HttpWebResponse webResponse = req.GetResponse(uri))
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        var responseString = streamReader.ReadToEnd();

                        var pattern = "<img src=\"(.*?)\">";
                        MatchCollection matches = Regex.Matches(responseString, pattern);

                        foreach (Match m in matches)
                            return String.Format("http://{0}/{1}", ip, m.Groups[1]);
                    }
                }
            }

            return null;
        }

        public void Stop()
        {
            _running = false;
        }

        public event Action<Image> OnImageArrived;
    }
}