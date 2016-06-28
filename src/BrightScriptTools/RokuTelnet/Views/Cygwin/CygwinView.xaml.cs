using System.Windows.Controls;
using RokuTelnet.Views.Output;

namespace RokuTelnet.Views.Cygwin
{
    /// <summary>
    /// Interaction logic for IOutputView.xaml
    /// </summary>
    public partial class CygwinView : UserControl, ICygwinView
    {
        public CygwinView()
        {
            InitializeComponent();
        }
    }
}
