using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using Prism.Commands;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Config
{
    public class ConfigViewModel : Prism.Mvvm.BindableBase, IConfigViewModel
    {
        private ConfigModel _model;
        private string _fileName;

        public ConfigViewModel(IConfigView view)
        {
            View = view;
            View.DataContext = this;

            Model = new ConfigModel();

            SaveCommand = new DelegateCommand(() =>
            {
                Save(_fileName);
                View.DialogResult = true;
                View.Close();
            });

            CancelCommand = new DelegateCommand(() =>
            {
                View.Close();
            });
        }

        public IConfigView View { get; set; }

        public ConfigModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(() => Model);
            }
        }

        public void Load(string filePath)
        {
            _fileName = filePath;

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    var content = sr.ReadToEnd();

                    Model = JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }
            else
            {
                var di = new DirectoryInfo(Path.GetDirectoryName(filePath));
                Model = new ConfigModel
                {
                    AppName = di.Name,
                    ArchiveName = di.Name.ToLower(),
                    BuildDirectory = "build",
                    Includes = new List<ConfigValueModel>
                    {
                        new ConfigValueModel { Value =  "source" },
                        new ConfigValueModel { Value =  "components" },
                        new ConfigValueModel { Value =  "images" }
                    }
                };
            }
        }

        public void Save(string filePath)
        {
            var content = JsonConvert.SerializeObject(Model);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var sw = new StreamWriter(filePath))
            {
                sw.Write(content);
            }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
    }
}