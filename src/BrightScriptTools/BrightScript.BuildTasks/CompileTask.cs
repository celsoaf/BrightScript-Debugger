using System;
using System.IO;
using System.Linq;
using BrightScriptTools.Compiler;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class CompileTask : BaseTask
    {
        public string BuildPath { get; set; }

        public ITaskItem[] Files { get; set; }

        protected override void InternalExecute()
        {
            if (Files != null)
            {
                foreach (var file in Files)
                {
                    Compile(file.ToString());
                }
            }
        }

        private void Compile(string file)
        {
            var path = Path.Combine(BuildPath, file);
            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                ErrorHandler handler = new ErrorHandler();
                // parse input args, and open input file
                Scanner scanner = new ASTScanner(stream);
                scanner.SetHandler(handler);

                Parser parser = new Parser(scanner, handler);
                if (!parser.Parse())
                {
                    handler.SortedErrorList().ToList().ForEach(e =>
                    {
                        Log.LogError("Compiler", "", "", file, e.Span.startLine, e.Span.startColumn, e.Span.endLine, e.Span.startColumn, e.Message);
                    });
                }
            }
        }
    }
}