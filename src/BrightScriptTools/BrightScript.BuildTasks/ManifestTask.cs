using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using NGit.Api;

namespace BrightScript.BuildTasks
{
    public class ManifestTask : BaseTask
    {
        private const string MANIFEST = "manifest";

        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }

        public string AppName { get; set; }
        [Required]
        public string MSBuildProjectName { get; set; }
        public string AppVersion { get; set; }

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

            if (!string.IsNullOrEmpty(AppVersion))
            {
                if (Regex.IsMatch(AppVersion, @"^(\d+\.)?(\d+\.)?(\*|\d+)$"))
                {
                    parts = AppVersion.Split('.');
                }
            }

            content = content.Replace("#MAJOR_VERSION#", parts.Length > 0 ? parts[0] : "1");
            content = content.Replace("#MINOR_VERSION#", parts.Length > 1 ? parts[1] : "0");
            content = content.Replace("#FIX_VERSION#", parts.Length > 2 ? parts[2] : "0");

            using (var sw = new StreamWriter(manifest))
                sw.Write(content);

            LogTaskMessage("Manifest processed");
        }
    }
}
