using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;
using RokuTelnet.Utils;

namespace RokuTelnet.Services.Screenshot
{
    public class ScreenshotService : IScreenshotService
    {
        private const string URL = "http://{0}//plugin_inspect";
        private const string OPTIONS_FILE = "deploy.json";
        private const string LAST_FOLDER_NAME = "lastFolder.json";

        private const int SLEEP_TIME = 1000;

        private volatile bool _running = false;

        private IEventAggregator _eventAggregator;

        public ScreenshotService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Start(string ip)
        {
            if (!_running)
            {
                _running = true;

                Task.Factory.StartNew(() =>
                {
                    while (_running)
                    {
                        try
                        {
                            var folder = LoadLastFolder();
                            if (!string.IsNullOrEmpty(folder))
                            {
                                var options = LoadModel(Path.Combine(folder, OPTIONS_FILE));

                                if (options != null)
                                {
                                    var url = GetImageUrl(ip, options);

                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        var image = GetImage(url, options);

                                        if(image!=null)
                                            _eventAggregator.GetEvent<ScreenshotEvent>().Publish(image);
                                    }
                                    else
                                    {
                                        _eventAggregator.GetEvent<ScreenshotEvent>().Publish(null);
                                    }
                                }
                                else
                                    Task.Delay(SLEEP_TIME).Wait();
                            }
                            else
                                Task.Delay(SLEEP_TIME).Wait();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                            Task.Delay(SLEEP_TIME).Wait();
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        private Image GetImage(string url, ConfigModel options)
        {
            var req = new DigestHttpWebRequest(options.User, options.Pass);

            Uri uri = new Uri(url);

            using (HttpWebResponse webResponse = req.GetResponse(uri))
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                return Image.FromStream(responseStream);
            }
        }

        private static string GetImageUrl(string ip, ConfigModel options)
        {
            var req = new DigestHttpWebRequest(options.User, options.Pass);

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

        private ConfigModel LoadModel(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }

            return null;
        }

        private string LoadLastFolder()
        {
            if (File.Exists(LAST_FOLDER_NAME))
            {
                using (var sr = new StreamReader(LAST_FOLDER_NAME))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<string>(content);
                }
            }

            return null;
        }
    }
}