using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using RokuTelnet.Services.Git;
using RokuTelnet.Utils;

namespace RokuTelnet.Services.Deploy
{
    public class DeployService : IDeployService
    {
        private const string OPTIONS_FILE = "deploy.json";
        private const string URL = "http://{0}//plugin_install";

        private readonly IGitService _gitService;

        public DeployService(IGitService gitService)
        {
            _gitService = gitService;
        }

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

                    ProcessManifest(outputFolder, folder);

                    Console.WriteLine("Manifest done");

                    var zipFile = Path.Combine(folder, options.Root.Element("archiveName").Value + ".zip");

                    CreateArchive(outputFolder, zipFile);

                    Console.WriteLine("Archive done");

                    DeployZip(zipFile, ip, options);

                    Console.WriteLine("Deploy complete");

                    File.Delete(zipFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DeployZip(string zipFile, string ip, XDocument options)
        {
            var responseString = string.Empty;

            var req = new DigestHttpWebRequest(
                options.Root.Element("username").Value,
                options.Root.Element("password").Value);

            req.Method = WebRequestMethods.Http.Post;

            Uri uri = new Uri(string.Format(URL, ip));

            using (HttpWebResponse webResponse = req.GetResponse(uri))
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            responseString = streamReader.ReadToEnd();
                        }
                    }
                }


            Console.WriteLine(responseString);
        }

        private void ProcessManifest(string outputFolder, string folder)
        {
            var version = _gitService.Describe(folder);

            if (!string.IsNullOrEmpty(version) && version.Contains("-"))
            {
                version = version.Split('-').First();
                if (Regex.IsMatch(version, @"^(\d+\.)?(\d+\.)?(\*|\d+)$"))
                {
                    var parts = version.Split('.');

                    var manifest = Path.Combine(outputFolder, "manifest");

                    var content = string.Empty;

                    using (var sr = new StreamReader(manifest))
                        content = sr.ReadToEnd();

                    content = content.Replace("#MAJOR_VERSION#", parts[0]);
                    content = content.Replace("#MINOR_VERSION#", parts[1]);
                    content = content.Replace("#FIX_VERSION#", parts[2]);

                    using (var sw = new StreamWriter(manifest))
                        sw.Write(content);
                }
            }
        }

        private void CreateArchive(string outputFolder, string zipFile)
        {
            if (File.Exists(zipFile))
                File.Delete(zipFile);

            ZipFile.CreateFromDirectory(outputFolder, zipFile);
        }

        private Dictionary<string, string> GetReplaces(XDocument options)
        {
            var replaces = options.Root.Elements("replaces")
                    .Select(x => new KeyValuePair<string, string>(x.Element("key")?.Value, x.Element("value")?.Value))
                    .ToDictionary(item => item.Key, item => item.Value);

            options.Root.Elements()
                    .Select(x => x.Name.LocalName)
                    .Where(n => n != "replaces")
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