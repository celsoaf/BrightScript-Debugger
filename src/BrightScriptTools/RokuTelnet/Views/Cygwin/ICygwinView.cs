namespace RokuTelnet.Views.Cygwin
{
    public interface ICygwinView
    {
        object DataContext { get; set; }

        void SetFocus();
        void SetCursorPosition();
    }
}