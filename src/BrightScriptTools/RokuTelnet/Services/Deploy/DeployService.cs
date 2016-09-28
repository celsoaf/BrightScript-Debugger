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
using Prism.Events;
using RokuTelnet.Events;
using RokuTelnet.Models;
using RokuTelnet.Services.Git;
using RokuTelnet.Utils;

namespace RokuTelnet.Services.Deploy
{
    public class DeployService : IDeployService
    {
        private const string URL = "http://{0}//plugin_install";

        private readonly IGitService _gitService;
        private IEventAggregator _eventAggregator;
        private volatile bool _running;

        public DeployService(IGitService gitService, IEventAggregator eventAggregator)
        {
            _gitService = gitService;
            _eventAggregator = eventAggregator;
        }

        public async Task Deploy(string ip, string folder, string optionsFile)
        {
            if (!_running)
            {
                _running = true;

                var zipFile = string.Empty;
                try
                {
                    var options = LoadModel(optionsFile);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Deploy started", 0, options));

                    var outputFolder = Path.Combine(folder, options.BuildDirectory);

                    _eventAggregator.GetEvent<BusyShowEvent>()
                        .Publish(GetBusy($"Copying files to '{options.BuildDirectory}", 1, options));

                    CopyFiles(folder, outputFolder, options);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Processing Manifest", 2, options));

                    ProcessManifest(outputFolder, folder, options);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Processing Container Registration", 3, options));

                    ProcessRegisterTypes(outputFolder, options);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Processing Replaces", 4, options));

                    ProcessReplaces(outputFolder, GetReplaces(options));

                    if (options.Optimize)
                    {
                        _eventAggregator.GetEvent<BusyShowEvent>()
                            .Publish(GetBusy("Processing Optimization", 5, options));

                        ProcessOptimize(outputFolder);
                    }

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Creating Archive", 6, options));

                    zipFile = Path.Combine(folder, options.ArchiveName + ".zip");

                    CreateArchive(outputFolder, zipFile);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy($"Deploying app to '{ip}'", 7, options));

                    DeployZip(zipFile, ip, options);

                    File.Delete(zipFile);
                    Directory.Delete(outputFolder, true);

                    _eventAggregator.GetEvent<BusyShowEvent>().Publish(GetBusy("Deploy complete", 8, options));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(zipFile) && File.Exists(zipFile))
                        File.Delete(zipFile);

                    Task.Delay(1000).ContinueWith(c => _eventAggregator.GetEvent<BusyHideEvent>().Publish(null));
                }

                _running = false;
            }
        }

        private void ProcessRegisterTypes(string outputFolder, ConfigModel options)
        {
            var types = new List<string>();
            var files = Directory.GetFiles(outputFolder, "*.brs", SearchOption.AllDirectories);
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

            options.ExtraConfigs.Add(new ConfigKeyValueModel { Key = "register_types", Value = sb.ToString() });
        }

        private BusyModel GetBusy(string message, double step, ConfigModel options)
        {
            return new BusyModel
            {
                Title = $"Deploying '{options.AppName}'",
                Message = message,
                Percentage = (step / 8d) * 100
            };
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
                contentNew = Regex.Replace(contentNew, @"\s?\/\s?", "/", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"\'%\-\-\'([\s\S]*?)\'\-\-%\'", "", RegexOptions.Multiline);

                contentNew = Regex.Replace(contentNew, @"^(\s)*\'.*", "", RegexOptions.Multiline);

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
                            Console.WriteLine("Deploy result: {0}", m.Groups[1]);
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

                    content = content.Replace("#MAJOR_VERSION#", parts.Length > 0 ? parts[0] : "0");
                    content = content.Replace("#MINOR_VERSION#", parts.Length > 1 ? parts[1] : "0");
                    content = content.Replace("#FIX_VERSION#", parts.Length > 2 ? parts[2] : "0");

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
                    .Where(c => c.Enable)
                    .ToDictionary(item => item.Key, item => item.Value);

            var elements = options.ExtraConfigs.ToList();

            options.GetType()
                .GetProperties()
                .ToList()
                .ForEach(p =>
                {
                    if (p.PropertyType == typeof(string))
                    {
                        elements.Add(new ConfigKeyValueModel { Key = p.Name, Value = (string)p.GetValue(options) });
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

                if (!Directory.Exists(dest))
                    Directory.CreateDirectory(dest);

                if (Directory.Exists(src))
                    CopyFolder(src, dest);
            }

            var excludes = options.Excludes;

            foreach (var value in excludes.Where(v => v.Value != null).Select(x => x.Value))
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
            foreach (string dirPath in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(src, dest));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(src, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(src, dest), true);
        }
    }
}