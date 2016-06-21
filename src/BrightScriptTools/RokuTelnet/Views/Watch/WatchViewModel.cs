namespace RokuTelnet.Views.Watch
{
    public class WatchViewModel : IWatchViewModel
    {
        public WatchViewModel(IWatchView view)
        {
            View = view;
            View.DataContext = this;
        }
         
        public IWatchView View { get; set; }
    }
}