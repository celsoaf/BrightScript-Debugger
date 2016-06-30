namespace RokuTelnet.Views.Output
{
    public interface IOutputView
    {
        object DataContext { get; set; }

        void SetFocus();
        void SetCursorPosition();
    }
}