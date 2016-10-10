using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class OptimizeTask : BaseTask
    {
        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }

        public string ProcessOptimize { get; set; }

        protected override void InternalExecute()
        {
            var output = Path.Combine(BuildPath, OutputPath);
            if (ProcessOptimize == "true")
            {
                Optimize(output);
                LogTaskMessage("Optimize complete");
            }
        }

        private void Optimize(string outputFolder)
        {
            foreach (string path in Directory.GetFiles(outputFolder, "*.brs", SearchOption.AllDirectories))
            {
                string contentOld = string.Empty;
                string contentNew = string.Empty;
                using (var sr = new StreamReader(path))
                    contentNew = contentOld = sr.ReadToEnd();

                contentNew = Regex.Replace(contentNew, @"\s?=\s?", "=", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?:\s?", ":", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?\+\s?", "+", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?-\s?", "-", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?\*\\s?", "*", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?\/\s?", "/", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"\'%\-\-\'([\s\S]*?)\'\-\-%\'", "", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"^(\s)*\'.*", "", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

                if (contentOld != contentNew)
                    using (var sw = new StreamWriter(path))
                        sw.Write(contentNew);
            }
        }
    }
}