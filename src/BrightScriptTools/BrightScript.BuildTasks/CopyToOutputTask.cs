using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class CopyToOutputTask : BaseTask
    {
        private const string MANIFEST_FILE = "app.manifest";
        private const string MANIFEST = "manifest";

        public string BuildPath { get; set; }
        public string OutputPath { get; set; }

        public ITaskItem[] CodeFiles { get; set; }
        public ITaskItem[] ImageFiles { get; set; }
        public ITaskItem[] ManifestFiles { get; set; }


        protected override void InternalExecute()
        {
            var output = Path.Combine(BuildPath, OutputPath);
            if (Directory.Exists(output))
                Directory.Delete(output, true);

            Directory.CreateDirectory(output);

            var files = new List<ITaskItem>();
            files.AddRange(CodeFiles);
            files.AddRange(ImageFiles);

            foreach (var file in files.Select(f=> f.ToString()))
            {
                var src = Path.Combine(BuildPath, file);
                var dest = Path.Combine(output, file);

                var dir = Path.GetDirectoryName(dest);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                
                File.Copy(src, dest);
            }

            if (ManifestFiles.Select(f => f.ToString()).Contains(MANIFEST_FILE))
            {
                var src = Path.Combine(BuildPath, MANIFEST_FILE);
                var dest = Path.Combine(output, MANIFEST);
                File.Copy(src, dest);
            }
            else
            {
                Log.LogError("{0} file does not exist", MANIFEST);
            }
        }
    }
}
