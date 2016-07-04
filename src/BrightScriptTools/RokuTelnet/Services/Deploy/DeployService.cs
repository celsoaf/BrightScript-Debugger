using System;
using System.IO;
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

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

            var excludes = options.Root.Elements("directoriesIncludeForDeploy");

            foreach (var value in excludes.Select(x => x.Value))
            {
                var dest = Path.Combine(outputFolder, value);

                Directory.Delete(dest, true);
            }
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