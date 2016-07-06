namespace RokuTelnet.Models
{
    public class ConfigValueModel : Prism.Mvvm.BindableBase
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(() => Value); }
        }
    }
}