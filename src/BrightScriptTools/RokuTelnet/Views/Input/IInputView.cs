namespace RokuTelnet.Views.Input
{
    public interface IInputView
    {
        object DataContext { get; set; }

        void SetFocus();
    }
}