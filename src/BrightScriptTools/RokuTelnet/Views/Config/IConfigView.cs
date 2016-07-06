namespace RokuTelnet.Views.Config
{
    public interface IConfigView
    {
        object DataContext { get; set; }

        bool? ShowDialog();
        bool? DialogResult { get; set; }
    }
}