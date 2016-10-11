using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using BrightScript.BuildTasks.Utils;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class DeployTask : BaseTask
    {
        private const string URL = "http://{0}//plugin_install";

        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }
        [Required]
        public string MSBuildProjectName { get; set; }
        [Required]
        public string BoxIP { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        protected override void InternalExecute()
        {
            var output = Path.Combine(BuildPath, OutputPath);
            var zipFile = Path.Combine(output, MSBuildProjectName + ".zip");

            DeployZip(zipFile);
        }

        private void DeployZip(string zipFile)
        {
            var responseString = string.Empty;

            var req = new DigestHttpWebRequest(UserName, Password);

            req.Method = WebRequestMethods.Http.Post;

            var formData = new MultipartFormData();
            formData.Add("mysubmit", "Install");
            formData.AddFile("archive", zipFile, "application/x-zip-compressed");
            req.PostData = formData.GetMultipartFormData();
            req.ContentType = formData.ContentType;


            Uri uri = new Uri(string.Format(URL, BoxIP));

            using (HttpWebResponse webResponse = req.GetResponse(uri))
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        responseString = streamReader.ReadToEnd();
                        
                        string pattern = "<font color=\"red\">(.*?)<\\/font>";
                        MatchCollection matches = Regex.Matches(responseString, pattern);

                        foreach (Match m in matches)
                            Log.LogError($"Deploy result: {m.Groups[1]}");
                    }
                }
            }
        }
    }
}