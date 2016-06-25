using System.Text;

namespace RokuTelnet.Views.Console
{
    public class ConsoleViewModel : Prism.Mvvm.BindableBase, IConsoleViewModel
    {
        private TextBoxOutputter _textBoxOutputter;
        private StringBuilder _stringBuilder;
        private string _text;

        public ConsoleViewModel(IConsoleView view)
        {
            View = view;
            View.DataContext = this;

            _stringBuilder = new StringBuilder();
            _textBoxOutputter = new TextBoxOutputter(_stringBuilder);

            _textBoxOutputter.TextChange += () => Text = _stringBuilder.ToString();

            System.Console.SetOut(_textBoxOutputter);
        }

        public IConsoleView View { get; set; }

        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(()=> Text); }
        }
    }
}