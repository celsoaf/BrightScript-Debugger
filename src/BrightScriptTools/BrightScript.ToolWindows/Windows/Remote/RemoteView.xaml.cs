//------------------------------------------------------------------------------
// <copyright file="RemoteToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace BrightScript.ToolWindows.Windows.Remote
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for RemoteToolWindowControl.
    /// </summary>
    public partial class RemoteView : UserControl, IRemoteView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteToolWindowControl"/> class.
        /// </summary>
        public RemoteView()
        {
            this.InitializeComponent();
        }
    }
}