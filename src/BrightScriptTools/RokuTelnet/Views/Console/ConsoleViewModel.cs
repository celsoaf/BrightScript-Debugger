namespace RokuTelnet.Views.Console
{
    public class ConsoleViewModel : IConsoleViewModel
    {
        public ConsoleViewModel(IConsoleView view)
        {
            View = view;
            View.DataContext = this;
        }

        public IConsoleView View { get; set; }
    }
}