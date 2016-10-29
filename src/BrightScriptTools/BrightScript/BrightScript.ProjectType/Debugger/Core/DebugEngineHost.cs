using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Core
{
    /// <summary>
    /// Static class providing telemetry reporting services to debug engines. Telemetry 
    /// reports go to Microsoft, and so in general this functionality should not be used 
    /// by non-Microsoft implemented debug engines.
    /// </summary>
    public static class HostTelemetry
    {
        /// <summary>
        /// Reports a telemetry event to Microsoft. This method is a nop in non-lab configurations.
        /// </summary>
        /// <param name="eventName">Name of the event. This should generally start with the 
        /// prefix 'VS/Diagnostics/Debugger/'</param>
        /// <param name="eventProperties">0 or more properties of the event. Property names 
        /// should generally start with the prefix 'VS.Diagnostics.Debugger.'</param>
        [Conditional("LAB")]
        public static void SendEvent(string eventName, params KeyValuePair<string, object>[] eventProperties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reports the current exception to Microsoft's telemetry service. 
        /// 
        /// *NOTE*: This should only be called from a 'catch(...) when' handler.
        /// </summary>
        /// <param name="currentException">Exception object to report.</param>
        /// <param name="engineName">Name of the engine reporting the exception. Ex:Microsoft.MIEngine</param>
        public static void ReportCurrentException(Exception currentException, string engineName)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides support for loading dependent assemblies using information from the configuration store.
    /// </summary>
    public static class HostLoader
    {
        /// <summary>
        /// Looks up the specified CLSID in the VS registry and loads it
        /// </summary>
        /// <param name="configStore">Registry root to lookup the type</param>
        /// <param name="clsid">CLSID to CoCreate</param>
        /// <returns>[Optional] loaded object. Null if the type is not registered, or points to a type that doesn't exist</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Co")]
        public static object VsCoCreateManagedObject(HostConfigurationStore configStore, Guid clsid)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This class provides marshalling helper methods to a debug engine. 
    /// 
    /// When run in Visual Studio, these methods deal with COM marshalling.
    /// 
    /// When run in Visual Studio code, these methods are stubs to allow the AD7 API to function without COM.
    /// </summary>
    public static class HostMarshal
    {
        /// <summary>
        /// Registers the specified code context if it isn't already registered and returns an IntPtr that can be
        /// used by the host to get back to the object.
        /// </summary>
        /// <param name="codeContext">Object to register</param>
        /// <returns>In VS, the IntPtr to a native COM object which can be returned to VS. In VS Code, an identifier
        /// that allows VS Code to get back to the object.</returns>
        public static IntPtr RegisterCodeContext(IDebugCodeContext2 codeContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtains a document position interface given the specified IntPtr of the document position.
        /// </summary>
        /// <param name="documentPositionId">In VS, the IUnknown pointer to QI for a document position. In VS Code,
        /// the identifier for the document position</param>
        /// <returns>Document position object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForInt")]
        public static IDebugDocumentPosition2 GetDocumentPositionForIntPtr(IntPtr documentPositionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtains a function position interface given the specified IntPtr of the location.
        /// </summary>
        /// <param name="locationId">In VS, the IUnknown pointer to QI for a function position. In VS Code,
        /// the identifier for the function position</param>
        /// <returns>Function position object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForInt")]
        public static IDebugFunctionPosition2 GetDebugFunctionPositionForIntPtr(IntPtr locationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtains an event callback interface that can be used to send events on any threads
        /// </summary>
        /// <param name="ad7Callback">The underlying event call back which was obtained from the port</param>
        /// <returns>In VS, a thread-safe wrapper on top of the underlying SDM event callback which allows
        /// sending events on any thread. In VS Code, this just returns the provided ad7Callback. </returns>
        public static IDebugEventCallback2 GetThreadSafeEventCallback(IDebugEventCallback2 ad7Callback)
        {
            return ad7Callback;
        }
    }

    /// <summary>
    /// Provides an abstraction over a wait loop that shows a dialog. In VS Code, this is currently
    /// stubbed out to wait without a dialog.
    /// </summary>
    public sealed class HostWaitLoop
    {
        private readonly string _message;

        /// <summary>
        /// Constructs a new HostWaitLoop
        /// </summary>
        /// <param name="message">The text of the wait dialog</param>
        public HostWaitLoop(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Wait for the specified handle to be signaled.
        /// </summary>
        /// <param name="handle">Handle to wait on.</param>
        /// <param name="cancellationSource">Cancellation token source to cancel if the user hits the cancel button.</param>
        public void Wait(WaitHandle handle, CancellationTokenSource cancellationSource)
        {
            handle.WaitOne();
        }

        /// <summary>
        /// Updates the progress of the dialog
        /// </summary>
        /// <param name="totalSteps">New total number of steps.</param>
        /// <param name="currentStep">The step that is currently finished.</param>
        /// <param name="progressText">Text describing the current progress.</param>
        public void SetProgress(int totalSteps, int currentStep, string progressText)
        {
            
        }
    }

    /// <summary>
    /// Provides an optional wait dialog to allow long running operations to be canceled. In VS Code,
    /// this is currently stubbed out to do nothing.
    /// </summary>
    public sealed class HostWaitDialog : IDisposable
    {
        private readonly string _format;
        private readonly string _caption;

        /// <summary>
        /// Construct a new instance of the HostWaitDialog class
        /// </summary>
        /// <param name="format">Format string used to create the wait dialog's body along with the 'item' argument to ShowWaitDialog.</param>
        /// <param name="caption">Caption of the dialog</param>
        public HostWaitDialog(string format, string caption)
        {
            _format = format;
            _caption = caption;
        }

        /// <summary>
        /// Updates the wait dialog for a new item.
        /// </summary>
        /// <param name="item">Item argument to when formatting the format string to create the wait dialog text.</param>
        public void ShowWaitDialog(string item)
        {
            
        }

        /// <summary>
        /// Ends the wait dialog
        /// </summary>
        public void EndWaitDialog()
        {
            
        }

        /// <summary>
        /// Ends the wait dialog
        /// </summary>
        public void Dispose()
        {
            EndWaitDialog();
        }
    }

    /// <summary>
    /// Provides interactions with the host's source workspace to locate and load any natvis files
    /// in the project.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Natvis")]
    public static class HostNatvisProject
    {
        /// <summary>
        /// Delegate which is fired to process a natvis file.
        /// </summary>
        /// <param name="path"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Natvis")]
        public delegate void NatvisLoader(string path);

        /// <summary>
        /// Searches the solution for natvis files, invoking the loader on any which are found.
        /// </summary>
        /// <param name="loader">Natvis loader method to invoke</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Natvis")]
        public static void FindNatvisInSolution(NatvisLoader loader)
        {
            
        }
    }

    /// <summary>
    /// Provides direct access to the underlying output window without going through debug events
    /// </summary>
    public static class HostOutputWindow
    {
        /// <summary>
        /// Write text to the Debug VS Output window pane directly. This is used to write information before the session create event.
        /// </summary>
        /// <param name="outputMessage">Message to write</param>
        public static void WriteLaunchError(string outputMessage)
        {
            throw new NotImplementedException();
        }
    }
}