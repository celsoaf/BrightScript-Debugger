using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class RegisterTypesTask : BaseTask
    {
        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }

        public string Generate { get; set; }

        [Output]
        public string RegisterTypes { get; set; }


        protected override void InternalExecute()
        {
            if (Generate == "true")
            {
                var output = Path.Combine(BuildPath, OutputPath);

                var types = new List<string>();
                var files = Directory.GetFiles(Path.Combine(output, "source"), "*.brs", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    using (var sr = new StreamReader(file))
                    {
                        var scriptName = Path.GetFileNameWithoutExtension(file);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                line = line.Trim().ToLower();
                                var tokens = line.Split(' ');
                                if (tokens[0] == "function")
                                {
                                    var fname = tokens[1];
                                    tokens = fname.Split('(');
                                    fname = tokens[0];
                                    if (!string.IsNullOrWhiteSpace(fname) && scriptName.ToLower() == fname)
                                    {
                                        types.Add(fname);
                                    }
                                }
                            }
                        }
                    }
                }

                var sb = new StringBuilder();
                types.ForEach(t =>
                {
                    sb.AppendFormat("ioc.registerType(\"{0}\", {0}){1}", t, Environment.NewLine);
                });

                RegisterTypes = sb.ToString();

                LogTaskMessage("RegisterTypes generated");
            }
        }
    }
}