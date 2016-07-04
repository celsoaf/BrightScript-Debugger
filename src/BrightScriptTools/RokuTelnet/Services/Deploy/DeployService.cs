using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace RokuTelnet.Services.Deploy
{
    public class DeployService : IDeployService
    {
        private const string OPTIONS_FILE = "deploy.json";

        public async Task Deploy(string ip, string folder)
        {
            try
            {
                using (var sr = new StreamReader(OPTIONS_FILE))
                {
                    var options = JsonConvert.DeserializeXNode(sr.ReadToEnd(), "root");

                    var outputFolder = Path.Combine(folder, options.Root.Element("buildDirectory").Value);

                    CopyFiles(folder, outputFolder, options);

                    Console.WriteLine("Copy done");

                    ProcessReplaces(outputFolder, GetReplaces(options));

                    Console.WriteLine("Replace done");

                    var zipFile = Path.Combine(folder, options.Root.Element("archiveName").Value + ".zip");

                    CreateArchive(outputFolder, zipFile);

                    Console.WriteLine("Archive done");

                    Console.WriteLine("Deploy complete");

                    File.Delete(zipFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateArchive(string outputFolder, string zipFile)
        {
            if(File.Exists(zipFile))
                File.Delete(zipFile);

            ZipFile.CreateFromDirectory(outputFolder, zipFile);
        }

        private Dictionary<string, string> GetReplaces(XDocument options)
        {
            var replaces = options.Root.Elements("replaces")
                    .Select(x => new KeyValuePair<string,string>(x.Element("key")?.Value, x.Element("value")?.Value))
                    .ToDictionary(item => item.Key, item => item.Value);

            options.Root.Elements()
                    .Select(x => x.Name.LocalName)
                    .Where(n=> n != "replaces")
                    .ToList()
                    .ForEach(k =>
                    {
                        replaces.ToList()
                            .ForEach(kv =>
                            {
                                var val = "{" + k + "}";
                                if (kv.Value.Contains(val))
                                    replaces[kv.Key] = kv.Value.Replace(val, options.Root.Element(k).Value);
                            });
                    });


            return replaces;
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

        private void CopyFiles(string folder, string outputFolder, XDocument options)
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);

            var includes = options.Root.Elements("directoriesIncludeForDeploy");

            foreach (var value in includes.Select(x => x.Value))
            {
                var src = Path.Combine(folder, value);
                var dest = Path.Combine(outputFolder, value);

                CopyFolder(src, dest);
            }

            var excludes = options.Root.Elements("directoriesExcludeForDeploy");

            foreach (var value in excludes.Select(x => x.Value))
            {
                var dest = Path.Combine(outputFolder, value);

                Directory.Delete(dest, true);
            }

            var manifestPath = Path.Combine(folder, "manifest");
            File.Copy(manifestPath, manifestPath.Replace(folder, outputFolder));
        }

        private static void CopyFolder(string src, string dest)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(src, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(src, dest));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(src, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(src, dest), true);
        }
    }
}