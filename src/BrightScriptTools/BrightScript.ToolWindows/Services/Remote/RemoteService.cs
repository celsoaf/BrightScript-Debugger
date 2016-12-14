using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using BrightScript.ToolWindows.Enums;
using BrightScript.ToolWindows.Models;
using Newtonsoft.Json;

namespace BrightScript.ToolWindows.Services.Remote
{
    public class RemoteService : IRemoteService
    {
        private const string URL = "http://{0}:8060/{1}/{2}";
        //private string _ip = "192.168.1.108";
        private int _port = 8060;

        public void Send(string ip, EventModel evt)
        {
            if (string.IsNullOrEmpty(ip)) return;

            try
            {
                using (var client = new WebClient())
                {
                    client.UploadString(GetUrl(ip, evt), "POST");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private string GetUrl(string ip, EventModel evt)
        {
            var url = string.Format(URL,
                ip,
                evt.EventType.ToString().ToLower(),
                evt.EventKey.ToString().ToLower());

            if (evt.EventKey == EventKey.Lit_ && evt.Args != null)
                url += HttpUtility.UrlEncode(evt.Args);

            return url;
        }


        public Task SendAsync(string ip, EventModel evt)
        {
            return Task.Factory.StartNew(() => Send(ip, evt));
        }
    }
}