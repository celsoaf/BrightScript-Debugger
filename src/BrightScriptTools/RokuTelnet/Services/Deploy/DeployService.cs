using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using RokuTelnet.Models;
using RokuTelnet.Services.Git;
using RokuTelnet.Utils;

namespace RokuTelnet.Services.Deploy
{
    public class DeployService : IDeployService
    {
        private const string URL = "http://{0}//plugin_install";

        private readonly IGitService _gitService;
        private volatile bool _running;

        public DeployService(IGitService gitService)
        {
            _gitService = gitService;
        }

        public async Task Deploy(string ip, string folder, string optionsFile)
        {
            if (!_running)
            {
                _running = true;

                try
                {
                    Console.WriteLine("Deploy started");

                    var options = LoadModel(optionsFile);

                    var outputFolder = Path.Combine(folder, options.BuildDirectory);

                    CopyFiles(folder, outputFolder, options);

                    Console.WriteLine("Copy done");

                    ProcessManifest(outputFolder, folder, options);

                    Console.WriteLine("Manifest done");

                    ProcessReplaces(outputFolder, GetReplaces(options));

                    Console.WriteLine("Replace done");

                    if (options.Optimize)
                    {
                        ProcessOptimize(outputFolder);

                        Console.WriteLine("Optimize done");
                    }

                    var zipFile = Path.Combine(folder, options.ArchiveName + ".zip");

                    CreateArchive(outputFolder, zipFile);

                    Console.WriteLine("Archive done");

                    DeployZip(zipFile, ip, options);

                    Console.WriteLine("Deploy complete");

                    File.Delete(zipFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                _running = false;
            }
        }

        private void ProcessOptimize(string outputFolder)
        {
            foreach (string path in Directory.GetFiles(outputFolder, "*.brs", SearchOption.AllDirectories))
            {
                string contentOld = string.Empty;
                string contentNew = string.Empty;
                using (var sr = new StreamReader(path))
                    contentNew = contentOld = sr.ReadToEnd();

                contentNew = Regex.Replace(contentNew, @"\s?=\s?", "=", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?:\s?", ":", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?\+\s?", "+", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?-\s?", "-", RegexOptions.Multiline);
                contentNew = Regex.Replace(contentNew, @"\s?\*\\s?", "*", RegexOptions.Multiline);
                //contentNew = Regex.Replace(contentNew, @"\s?/\s?", "/", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"\'%\-\-\'([\s\S]*?)\'\-\-%\'", "", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, "(?!\")\'*(\r\n|\n|\r)", "\r\n", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

                if (contentOld != contentNew)
                    using (var sw = new StreamWriter(path))
                        sw.Write(contentNew);
            }
        }

        private ConfigModel LoadModel(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    var content = sr.ReadToEnd();

                    return JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }

            return null;
        }

        private void DeployZip(string zipFile, string ip, ConfigModel options)
        {
            var responseString = string.Empty;

            var req = new DigestHttpWebRequest(options.User, options.Pass);

            req.Method = WebRequestMethods.Http.Post;

            var formData = new MultipartFormData();
            formData.Add("mysubmit", "Install");
            formData.AddFile("archive", zipFile, "application/x-zip-compressed");
            req.PostData = formData.GetMultipartFormData();
            req.ContentType = formData.ContentType;


            Uri uri = new Uri(string.Format(URL, ip));

            using (HttpWebResponse webResponse = req.GetResponse(uri))
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        responseString = streamReader.ReadToEnd();

                        string pattern = "<font color=\"red\">(.*?)<\\/font>";
                        MatchCollection matches = Regex.Matches(responseString, pattern);

                        foreach (Match m in matches)
                            Console.WriteLine("result: {0}", m.Groups[1]);
                    }
                }
            }
        }

        private void ProcessManifest(string outputFolder, string folder, ConfigModel options)
        {
            var version = _gitService.Describe(folder);

            if (!string.IsNullOrEmpty(version) && version.Contains("-"))
            {
                version = version.Split('-').First();
                if (Regex.IsMatch(version, @"^(\d+\.)?(\d+\.)?(\*|\d+)$"))
                {
                    options.ExtraConfigs.Add(new ConfigKeyValueModel { Key = "version", Value = version });

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

        private Dictionary<string, string> GetReplaces(ConfigModel options)
        {
            var replaces = options.Replaces
                    .ToDictionary(item => item.Key, item => item.Value);

            var elements = options.ExtraConfigs.ToList();

            options.GetType()
                .GetProperties()
                .ToList()
                .ForEach(p =>
                {
                    if (p.PropertyType == typeof(string))
                    {
                        elements.Add(new ConfigKeyValueModel {Key = p.Name, Value = (string)p.GetValue(options)});
                    }
                });

            var dic = elements.ToDictionary(item => item.Key, item => item.Value);
            
            dic.Keys.ForEach(k =>
                    {
                        replaces.ToList()
                            .ForEach(kv =>
                            {
                                var val = "{" + k + "}";
                                if (kv.Value.Contains(val))
                                    replaces[kv.Key] = kv.Value.Replace(val, dic[k]);
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

        private void CopyFiles(string folder, string outputFolder, ConfigModel options)
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);

            var includes = options.Includes;

            foreach (var value in includes.Select(x => x.Value))
            {
                var src = Path.Combine(folder, value);
                var dest = Path.Combine(outputFolder, value);

                if (Directory.Exists(src))
                    CopyFolder(src, dest);
            }

            var excludes = options.Excludes;

            foreach (var value in excludes.Select(x => x.Value))
            {
                var dest = Path.Combine(outputFolder, value);

                if (Directory.Exists(dest))
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