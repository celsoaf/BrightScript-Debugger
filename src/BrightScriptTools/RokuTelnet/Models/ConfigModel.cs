using System.Collections.Generic;

namespace RokuTelnet.Models
{
    public class ConfigModel : Prism.Mvvm.BindableBase
    {
        private string _user;
        private string _pass;
        private string _appName;
        private string _archiveName;
        private string _buildDirectory;
        private bool _optimize;
        private List<ConfigValueModel> _includes = new List<ConfigValueModel>();
        private List<ConfigValueModel> _excludes = new List<ConfigValueModel>();
        private List<ConfigKeyValueModel> _extraConfigs = new List<ConfigKeyValueModel>();
        private List<ConfigReplaceModel> _replaces = new List<ConfigReplaceModel>();

        public string User
        {
            get { return _user; }
            set { _user = value; OnPropertyChanged(()=> User); }
        }

        public string Pass
        {
            get { return _pass; }
            set { _pass = value; OnPropertyChanged(()=> Pass); }
        }

        public string AppName
        {
            get { return _appName; }
            set { _appName = value; OnPropertyChanged(()=> AppName); }
        }

        public string ArchiveName
        {
            get { return _archiveName; }
            set { _archiveName = value; OnPropertyChanged(()=> ArchiveName); }
        }

        public string BuildDirectory
        {
            get { return _buildDirectory; }
            set { _buildDirectory = value; OnPropertyChanged(()=> BuildDirectory); }
        }

        public bool Optimize
        {
            get { return _optimize; }
            set { _optimize = value; OnPropertyChanged(()=> Optimize); }
        }

        public List<ConfigValueModel> Includes
        {
            get { return _includes; }
            set { _includes = value; OnPropertyChanged(()=> Includes); }
        }

        public List<ConfigValueModel> Excludes
        {
            get { return _excludes; }
            set { _excludes = value; OnPropertyChanged(()=> Excludes); }
        }

        public List<ConfigKeyValueModel> ExtraConfigs
        {
            get { return _extraConfigs; }
            set { _extraConfigs = value; OnPropertyChanged(()=> ExtraConfigs); }
        }

        public List<ConfigReplaceModel> Replaces
        {
            get { return _replaces; }
            set { _replaces = value; OnPropertyChanged(()=> Replaces); }
        }
    }
}