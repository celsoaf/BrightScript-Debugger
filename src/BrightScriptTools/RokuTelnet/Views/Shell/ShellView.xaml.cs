﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using RokuTelnet.Views.Output;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace RokuTelnet.Views.Shell
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IShellView
    {
        public const string LAYOUT_FILE = "layout.xml";

        public ShellView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var serializer = new XmlLayoutSerializer(DockingManager);
                //serializer.Deserialize(LAYOUT_FILE);
            };

            Closing += (s, e) =>
            {
                var serializer = new XmlLayoutSerializer(DockingManager);
                serializer.Serialize(LAYOUT_FILE);
            };
        }

        private void LayoutElement_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedContentIndex" && DataContext != null)
            {
                ((IShellViewModel) DataContext).SelectedIndex = telnetPanel.SelectedContentIndex;
                if (telnetPanel.SelectedContentIndex >= 0 &&
                    telnetPanel.SelectedContentIndex < telnetPanel.ChildrenCount)
                {
                    var content = telnetPanel.Children[telnetPanel.SelectedContentIndex].Content as ContentControl;
                    var output = content.Content as IOutputView;
                    if (output != null)
                    {
                        ((IOutputViewModel)output.DataContext).SetActive();
                    }
                }
            }
        }
    }
}
