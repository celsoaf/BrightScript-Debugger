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

namespace RokuTelnet.Views.Output
{
    /// <summary>
    /// Interaction logic for IOutputView.xaml
    /// </summary>
    public partial class OutputView : UserControl, IOutputView
    {
        public OutputView()
        {
            InitializeComponent();
        }

        public void SetFocus()
        {
            InputBlock.Focus();
        }
    }
}
