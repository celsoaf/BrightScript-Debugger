namespace RokuTelnet.Models
{
    public class ConfigKeyModel : Prism.Mvvm.BindableBase
    {
        private string _key;
        private string _value;

        public string Key
        {
            get { return _key; }
            set { _key = value; OnPropertyChanged(()=> Key); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(()=> Value); }
        }
    }
}