using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class CopyToOutputTask : BaseTask
    {
        public string BuildPath { get; set; }
        public string OutputPath { get; set; }

        public ITaskItem[] CodeFiles { get; set; }
        public ITaskItem[] ImageFiles { get; set; }
        public ITaskItem[] ManifestFiles { get; set; }


        protected override void InternalExecute()
        {
            Log.LogError(BuildPath);
            Log.LogError(OutputPath);
        }
    }
}
