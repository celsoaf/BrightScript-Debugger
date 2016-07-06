using System.Collections.ObjectModel;
using Prism.Commands;
using RokuTelnet.Models;

namespace RokuTelnet.Views.Config
{
    public class ConfigViewModel : Prism.Mvvm.BindableBase, IConfigViewModel
    {
        private ConfigModel _model;
        private ObservableCollection<string> _includes;
        private ObservableCollection<string> _excludes;
        private ObservableCollection<ConfigKeyModel> _extraConfigs;
        private ObservableCollection<ConfigKeyModel> _replaces;

        public ConfigViewModel(IConfigView view)
        {
            View = view;
            View.DataContext = this;

            Model = new ConfigModel();
        }

        public IConfigView View { get; set; }

        public ConfigModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(() => Model);
                UpdateLists();
            }
        }

        private void UpdateLists()
        {
            if (Model != null)
            {
                Includes = new ObservableCollection<string>(Model.Includes);
                Excludes = new ObservableCollection<string>(Model.Excludes);
                ExtraConfigs = new ObservableCollection<ConfigKeyModel>(Model.ExtraConfigs);
                Replaces = new ObservableCollection<ConfigKeyModel>(Model.Replaces);
            }
            else
            {
                Includes = new ObservableCollection<string>();
                Excludes = new ObservableCollection<string>();
                ExtraConfigs = new ObservableCollection<ConfigKeyModel>();
                Replaces = new ObservableCollection<ConfigKeyModel>();
            }
        }

        public void Load(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public void Save(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public ObservableCollection<string> Includes
        {
            get { return _includes; }
            set { _includes = value; OnPropertyChanged(() => Includes); }
        }

        public ObservableCollection<string> Excludes
        {
            get { return _excludes; }
            set { _excludes = value; OnPropertyChanged(() => Excludes); }
        }

        public ObservableCollection<ConfigKeyModel> ExtraConfigs
        {
            get { return _extraConfigs; }
            set { _extraConfigs = value; OnPropertyChanged(() => ExtraConfigs); }
        }

        public ObservableCollection<ConfigKeyModel> Replaces
        {
            get { return _replaces; }
            set { _replaces = value; OnPropertyChanged(() => Replaces); }
        }
    }
}