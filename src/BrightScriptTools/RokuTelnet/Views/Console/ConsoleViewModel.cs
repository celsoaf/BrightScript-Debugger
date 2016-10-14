using System.IO;
using System.Text;
using Prism.Events;
using RokuTelnet.Events;

namespace RokuTelnet.Views.Console
{
    public class ConsoleViewModel : Prism.Mvvm.BindableBase, IConsoleViewModel
    {
        private TextBoxOutputter _textBoxOutputter;
        private StringBuilder _stringBuilder;
        private string _text;
        private bool _showError;
        private TextWriter _errorTextWriter;
        private readonly IEventAggregator _eventAggregator;

        public ConsoleViewModel(IConsoleView view, IEventAggregator eventAggregator)
        {
            View = view;
            View.DataContext = this;

            _eventAggregator = eventAggregator;
            _stringBuilder = new StringBuilder();
            _textBoxOutputter = new TextBoxOutputter(_stringBuilder);

            _textBoxOutputter.TextChange += () => Text = _stringBuilder.ToString();

            System.Console.SetOut(_textBoxOutputter);

            _errorTextWriter = System.Console.Error;

            _eventAggregator.GetEvent<ClearLogsEvent>().Subscribe(obj =>
            {
                _stringBuilder.Clear();
                Text = _stringBuilder.ToString();
            });
        }

        public IConsoleView View { get; set; }

        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(()=> Text); }
        }

        public bool ShowError
        {
            get { return _showError; }
            set
            {
                _showError = value;
                
                if(_showError)
                    System.Console.SetError(_textBoxOutputter);
                else
                    System.Console.SetError(_errorTextWriter);
            }
        }
    }
}