using System;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class CompileTask : BaseTask
    {
        public ITaskItem[] Files { get; set; }

        protected override void InternalExecute()
        {
            if (Files != null)
            {
                foreach (var file in Files)
                {
                    Log.LogError(file.ToString());
                }
            }
        }
    }
}