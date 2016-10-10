using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using NGit.Api;

namespace BrightScript.BuildTasks
{
    public class GitVersionTask : BaseTask
    {
        [Required]
        public string BuildPath { get; set; }

        public string Generate { get; set; }

        [Output]
        public string AppVersion { get; set; }

        protected override void InternalExecute()
        {
            if (Generate == "true")
            {
                AppVersion = GetVersion(BuildPath);

                LogTaskMessage($"Version {AppVersion}");
            }
        }

        private string GetVersion(string path)
        {
            try
            {
                var repo = Git.Open(path);

                var tags = repo.TagList().Call().Select(r => r.ToString());

                var versions = new List<Version>();
                foreach (var tag in tags)
                {
                    var match = Regex.Match(tag, @"(\d+\.)(\d+\.)?(\*|\d+)");
                    if (match.Success)
                    {
                        var version = Version.Parse(match.Value);
                        versions.Add(version);
                    }
                }

                if (versions.Count > 0)
                    return versions.OrderByDescending(v => v).First().ToString();
            }
            catch (Exception ex)
            {
                LogTaskWarning(ex.Message);
            }

            return "Unknow version";
        }
    }
}