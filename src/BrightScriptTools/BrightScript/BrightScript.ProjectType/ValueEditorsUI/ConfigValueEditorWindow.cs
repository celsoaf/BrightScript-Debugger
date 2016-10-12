using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace BrightScript.ValueEditorsUI
{
    public partial class ConfigValueEditorWindow : Form
    {
        public ConfigValueEditorWindow()
        {
            InitializeComponent();
        }

        public void SetData(DataTable config, DataTable replaces)
        {
            configParamsGrid.DataSource = config;
            replacesGrid.DataSource = replaces;
        }

        private void okBnt_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBnt_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
