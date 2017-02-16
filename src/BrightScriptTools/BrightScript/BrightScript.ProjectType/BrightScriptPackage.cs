//------------------------------------------------------------------------------
// <copyright file="BrightScriptPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Register;
using BrightScript.SharedProject;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace BrightScript
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
    // VSConstants.UICONTEXT_NoSolution
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(BrightScriptPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideDebugEngine("BrightScript Debugging", typeof(AD7ProgramProvider), typeof(AD7Engine), AD7Engine.DebugEngineId, setNextStatement: false, hitCountBp: true, justMyCodeStepping: false)]
    // Keep declared exceptions in sync with those given default values in NodeDebugger.GetDefaultExceptionTreatments()
    [ProvideBsDebugException()]
    [ProvideBsDebugException("Error")]
    [ProvideService(typeof(UIThreadBase))]
    [ProvideBindingPath]
    public sealed class BrightScriptPackage : Package
    {
        /// <summary>
        /// The GUID for this package.
        /// </summary>
        public const string PackageGuid = "ec697b03-fb4a-4218-b12b-31187659df23";

        /// <summary>
        /// The GUID for this project type.  It is unique with the project file extension and
        /// appears under the VS registry hive's Projects key.
        /// </summary>
        public const string ProjectTypeGuid = "c7df651a-fd18-4ea9-87a6-a7ad191a493e";

        /// <summary>
        /// The file extension of this project type.  No preceding period.
        /// </summary>
        public const string ProjectExtension = "brsproj";

        /// <summary>
        /// The default namespace this project compiles with, so that manifest
        /// resource names can be calculated for embedded resources.
        /// </summary>
        internal const string DefaultNamespace = "BrightScript";

        /// <summary>
        /// BrightScriptPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "35e2bd07-a62c-467c-b724-640e3b18d4b1";

        internal static BrightScriptPackage Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightScriptPackage"/> class.
        /// </summary>
        public BrightScriptPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            Debug.Assert(Instance == null, "BrightScriptPackage created multiple times");
            Instance = this;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            UIThread.EnsureService(this);

            base.Initialize();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            System.Threading.Tasks.Task.Factory.StartNew(CopyProjectFiles);
        }

        private void CopyProjectFiles()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assembly = Assembly.LoadFile(Path.Combine(dir, "BrightScript.BuildSystem.dll"));
            var appData = Environment.GetEnvironmentVariable("LocalAppData");
            appData += @"\CustomProjectSystems\BrightScript";
            var appDataRules = Path.Combine(appData, "Rules");
            if (!Directory.Exists(appDataRules))
                Directory.CreateDirectory(appDataRules);

            var mapper = new Dictionary<string, string>
            {
                { "DeployedBuildSystem", appData },
                { "Rules", appDataRules }
            };

            foreach (var name in assembly.GetManifestResourceNames())
            {
                var parts = name.Split('.');
                var folder = parts[2];
                var file = String.Join(".", parts.Skip(3));

                if (mapper.ContainsKey(folder))
                {
                    var filePath = Path.Combine(mapper[folder], file);
                    if (!File.Exists(filePath))
                        using (var s = assembly.GetManifestResourceStream(name))
                            using (var f = File.Create(filePath))
                                s.CopyTo(f);
                }
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var parts = args.Name.Split(',');
            var file = Path.Combine(dir, parts[0] + ".dll");

            if (!File.Exists(file))
                file = Path.ChangeExtension(file, ".exe");
            if (File.Exists(file))
                return Assembly.LoadFrom(file);

            return null;
        }

        #endregion
    }
}
