﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RokuTelnet.Views.Remote
{
    /// <summary>
    /// Interaction logic for RemoteView.xaml
    /// </summary>
    public partial class RemoteView : UserControl, IRemoteView
    {
        public RemoteView()
        {
            InitializeComponent();
        }

        private void RemoteView_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Input.Focus();
        }
    }
}
