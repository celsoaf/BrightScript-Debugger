using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NGit.Api;

namespace BrightScript.BuildTasks
{
    public class ManifestTask : BaseTask
    {
        private const string MANIFEST = "manifest";

        public string BuildPath { get; set; }
        public string OutputPath { get; set; }

        public string AppName { get; set; }
        public string MSBuildProjectName { get; set; }
        public string GitVersion { get; set; }

        protected override void InternalExecute()
        {
            var manifest = Path.Combine(BuildPath, OutputPath, "manifest");

            var content = string.Empty;

            using (var sr = new StreamReader(manifest))
                content = sr.ReadToEnd();

            if (!string.IsNullOrEmpty(AppName))
                content = content.Replace("#APP_NAME#", AppName);
            else
                content = content.Replace("#APP_NAME#", MSBuildProjectName);

            var parts = new string[0];

            if (GitVersion == "true")
            {
                var version = GetGitDescribe(BuildPath);

                if (!string.IsNullOrEmpty(version) && version.Contains("-"))
                {
                    version = version.Split('-').First();
                    if (Regex.IsMatch(version, @"^(\d+\.)?(\d+\.)?(\*|\d+)$"))
                    {
                        parts = version.Split('.');
                    }
                }
            }

            content = content.Replace("#MAJOR_VERSION#", parts.Length > 0 ? parts[0] : "1");
            content = content.Replace("#MINOR_VERSION#", parts.Length > 1 ? parts[1] : "0");
            content = content.Replace("#FIX_VERSION#", parts.Length > 2 ? parts[2] : "0");

            using (var sw = new StreamWriter(manifest))
                sw.Write(content);

            
        }

        public string GetGitDescribe(string path)
        {
            try
            {
                var repo = Git.Open(BuildPath);

                return null;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return "Unknow version";
            }
        }
    }
}
