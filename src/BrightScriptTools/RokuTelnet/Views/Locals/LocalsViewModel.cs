namespace RokuTelnet.Views.Locals
{
    public class LocalsViewModel : ILocalsViewModel
    {
        public LocalsViewModel(ILocalsView view)
        {
            View = view;
            View.DataContext = this;
        }

        public ILocalsView View { get; set; }
    }
}