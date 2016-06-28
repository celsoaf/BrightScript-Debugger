using System.Windows.Controls;
using System.Windows.Input;
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

        private void CygwinView_OnMouseEnter(object sender, MouseEventArgs e)
        {
            InputBlock.Focus();
        }
    }
}
