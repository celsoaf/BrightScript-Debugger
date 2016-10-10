using System.IO;
using System.IO.Compression;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class ArchiveTask : BaseTask
    {
        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }
        [Required]
        public string MSBuildProjectName { get; set; }

        protected override void InternalExecute()
        {
            var output = Path.Combine(BuildPath, OutputPath);

            var zipFile = Path.Combine(BuildPath, MSBuildProjectName + ".zip");

            CreateArchive(output, zipFile);

            var dest = Path.Combine(output, MSBuildProjectName + ".zip");
            File.Move(zipFile, dest);

            LogTaskMessage("Archive complete");
        }

        private void CreateArchive(string outputFolder, string zipFile)
        {
            if (File.Exists(zipFile))
                File.Delete(zipFile);

            ZipFile.CreateFromDirectory(outputFolder, zipFile);
        }
    }
}