namespace RokuTelnet.Views.Config
{
    public interface IConfigView
    {
        object DataContext { get; set; }

        bool? ShowDialog();
        void Close();
        bool? DialogResult { get; set; }
    }
}