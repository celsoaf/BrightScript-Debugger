using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Register;
using Microsoft.VisualStudio.Shell;

namespace BrightScript.Debugger
{
    [ProvideDebugEngine("BrightScript Debugging", typeof(AD7ProgramProvider), typeof(AD7Engine), AD7Engine.DebugEngineId, setNextStatement: false, hitCountBp: true, justMyCodeStepping: false)]
    // Keep declared exceptions in sync with those given default values in NodeDebugger.GetDefaultExceptionTreatments()
    [ProvideBsDebugException()]
    [ProvideBsDebugException("Error")]
    // VS2015's exception manager uses a different nesting structure, so it's necessary to register Error explicitly.
    [ProvideBsDebugException("Error", "Error")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Description("A custom project type based on CPS")]
    [Guid(DebuggerPackage.PackageGuid)]
    public class DebuggerPackage : Package
    {
        /// <summary>
        /// The GUID for this package.
        /// </summary>
        public const string PackageGuid = "4B4635EA-CD84-4D81-B96F-F25A2A781EEF";

        public DebuggerPackage()
        {
            Console.WriteLine("TTT");
        }
    }
}