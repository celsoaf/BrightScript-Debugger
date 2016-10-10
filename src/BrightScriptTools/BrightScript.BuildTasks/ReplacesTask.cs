using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;

namespace BrightScript.BuildTasks
{
    public class ReplacesTask : BaseTask
    {
        [Required]
        public string BuildPath { get; set; }
        [Required]
        public string OutputPath { get; set; }

        public string ReplaceConfigs { get; set; }

        public string AppVersion { get; set; }
        public string RegisterTypes { get; set; }

        protected override void InternalExecute()
        {
            var output = Path.Combine(BuildPath, OutputPath);
            if (!string.IsNullOrWhiteSpace(ReplaceConfigs))
            {
                var configs = new List<Tuple<string, string>>();
                var replaces = new List<Tuple<string, string, bool>>();
                var tables = ReplaceConfigs.Split('|');
                if (tables.Length == 2)
                {
                    tables[0].Split(';')
                        .ToList()
                        .ForEach(s =>
                        {
                            var parts = s.Split(',');
                            if (parts.Length==2)
                                configs.Add(new Tuple<string, string>(parts[0], parts[1]));
                        });
                    tables[1].Split(';')
                        .ToList()
                        .ForEach(s =>
                        {
                            var parts = s.Split(',');
                            if(parts.Length==3)
                                replaces.Add(new Tuple<string, string, bool>(parts[0], parts[1], bool.Parse(parts[2])));
                        });
                }

                if(!string.IsNullOrWhiteSpace(AppVersion))
                    configs.Add(new Tuple<string, string>("version", AppVersion));

                if (!string.IsNullOrWhiteSpace(RegisterTypes))
                    configs.Add(new Tuple<string, string>("register_types", RegisterTypes));

                ProcessReplaces(output, GetReplaces(configs, replaces));

                LogTaskMessage("Replaces processed");
            }
        }

        private Dictionary<string, string> GetReplaces(List<Tuple<string, string>> configs, List<Tuple<string, string, bool>> replaces)
        {
            var dic = replaces.Where(t=>t.Item3)
                            .ToDictionary(i => i.Item1, i=> i.Item2);

            configs.ForEach(t =>
                {
                    dic.ToList()
                        .ForEach(kv =>
                        {
                            var val = "{" + t.Item1 + "}";
                            if (kv.Value.Contains(val))
                                dic[kv.Key] = kv.Value.Replace(val, t.Item2);
                        });
                });


            return dic;
        }

        private void ProcessReplaces(string outputFolder, Dictionary<string, string> replaces)
        {
            foreach (string path in Directory.GetFiles(outputFolder, "*.*", SearchOption.AllDirectories))
            {
                string contentOld = string.Empty;
                string contentNew = string.Empty;
                using (var sr = new StreamReader(path))
                    contentNew = contentOld = sr.ReadToEnd();

                foreach (var item in replaces)
                {
                    contentNew = contentNew.Replace(item.Key, item.Value);
                }

                if (contentOld != contentNew)
                    using (var sw = new StreamWriter(path))
                        sw.Write(contentNew);
            }
        }

    }
}