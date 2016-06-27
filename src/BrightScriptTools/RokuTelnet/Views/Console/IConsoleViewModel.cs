namespace RokuTelnet.Views.Console
{
    public interface IConsoleViewModel
    {
        IConsoleView View { get; set; }

        string Text { get; set; }

        bool ShowError { get; set; }
    }
}