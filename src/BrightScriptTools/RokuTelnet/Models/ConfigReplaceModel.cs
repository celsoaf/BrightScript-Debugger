namespace RokuTelnet.Models
{
    public class ConfigReplaceModel : Prism.Mvvm.BindableBase
    {
        private string _key;
        private string _value;
        private bool _enable;

        public string Key
        {
            get { return _key; }
            set { _key = value; OnPropertyChanged(() => Key); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(() => Value); }
        }

        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; OnPropertyChanged(()=> Enable); }
        }
    }
}