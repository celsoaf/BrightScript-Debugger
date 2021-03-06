﻿//------------------------------------------------------------------------------
// <copyright file="ToolWindowsPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using BrightScript.ToolWindows.Windows.Remote;
using BrightScript.ToolWindows.Windows.Screenshot;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace BrightScript.ToolWindows
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(ToolWindowsPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(RemoteToolWindow), Style = VsDockStyle.Tabbed, Window = ToolWindowGuids80.SolutionExplorer)]
    [ProvideToolWindowVisibility(typeof(RemoteToolWindow), VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
    [ProvideToolWindow(typeof(ScreenshotToolWindow), Style = VsDockStyle.Tabbed, Window = ToolWindowGuids80.Locals)]
    public sealed class ToolWindowsPackage : Package
    {
        /// <summary>
        /// ToolWindowsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "f4defd8b-9609-47b8-a993-eac56181f5bf";

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowsPackage"/> class.
        /// </summary>
        public ToolWindowsPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            BrightScript.ToolWindows.Windows.Remote.RemoteToolWindowCommand.Initialize(this);
            BrightScript.ToolWindows.Windows.Screenshot.ScreenshotToolWindowCommand.Initialize(this);
        }

        #endregion
    }
}
