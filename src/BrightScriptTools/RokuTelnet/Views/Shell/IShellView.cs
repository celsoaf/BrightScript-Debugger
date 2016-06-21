namespace RokuTelnet.Views.Shell
{
    public interface IShellView
    {
        object DataContext { get; set; }
        void Show();
    }
}