﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BrightScript.Debugger.Core {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class MICoreResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MICoreResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BrightScript.Debugger.Core.MICoreResources", typeof(MICoreResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Required attribute &apos;{0}&apos; is missing or has an invalid value..
        /// </summary>
        internal static string Error_BadRequiredAttribute {
            get {
                return ResourceManager.GetString("Error_BadRequiredAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Both &apos;{0}&apos; and &apos;{1}&apos; cannot be specified at the same time..
        /// </summary>
        internal static string Error_CannotSpecifyBoth {
            get {
                return ResourceManager.GetString("Error_CannotSpecifyBoth", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internal error in MIEngine. Exception of type &apos;{0}&apos; was thrown.
        ///{1}.
        /// </summary>
        internal static string Error_CorruptingException {
            get {
                return ResourceManager.GetString("Error_CorruptingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to establish a connection to {0}. Debug output may contain more information..
        /// </summary>
        internal static string Error_DebuggerInitializeFailed_NoStdErr {
            get {
                return ResourceManager.GetString("Error_DebuggerInitializeFailed_NoStdErr", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to establish a connection to {0}. The following message was written to stderr:
        ///{1}.
        /// </summary>
        internal static string Error_DebuggerInitializeFailed_StdErr {
            get {
                return ResourceManager.GetString("Error_DebuggerInitializeFailed_StdErr", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Debug server process failed to initialize..
        /// </summary>
        internal static string Error_DebugServerInitializationFailed {
            get {
                return ResourceManager.GetString("Error_DebugServerInitializationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception while processing MIEngine operation. {0}. If the problem continues restart debugging..
        /// </summary>
        internal static string Error_ExceptionInOperation {
            get {
                return ResourceManager.GetString("Error_ExceptionInOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing modules from the target process.
        ///Modules: {0}
        ///Error: {1}.
        /// </summary>
        internal static string Error_ExceptionProcessingModules {
            get {
                return ResourceManager.GetString("Error_ExceptionProcessingModules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command elements must have a body (ex: &lt;Command&gt;gdb_command_here&lt;/Command&gt;)..
        /// </summary>
        internal static string Error_ExpectedCommandBody {
            get {
                return ResourceManager.GetString("Error_ExpectedCommandBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while trying to enter break state. Debugging will now stop. {0}.
        /// </summary>
        internal static string Error_FailedToEnterBreakState {
            get {
                return ResourceManager.GetString("Error_FailedToEnterBreakState", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Launch options string provided by the project system is invalid. {0}.
        /// </summary>
        internal static string Error_InvalidLaunchOptions {
            get {
                return ResourceManager.GetString("Error_InvalidLaunchOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid path to core dump file &apos;{0}&apos;. File must be a valid file name that exists on the computer..
        /// </summary>
        internal static string Error_InvalidLocalCoreDumpPath {
            get {
                return ResourceManager.GetString("Error_InvalidLocalCoreDumpPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid path to executable file path &apos;{0}&apos;. File must be a valid file name that exists..
        /// </summary>
        internal static string Error_InvalidLocalExePath {
            get {
                return ResourceManager.GetString("Error_InvalidLocalExePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of miDebuggerPath is invalid.
        /// </summary>
        internal static string Error_InvalidMiDebuggerPath {
            get {
                return ResourceManager.GetString("Error_InvalidMiDebuggerPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device App Launcher {0} could not be found..
        /// </summary>
        internal static string Error_LauncherNotFound {
            get {
                return ResourceManager.GetString("Error_LauncherNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} exited unexpectedly. Debugging will now abort..
        /// </summary>
        internal static string Error_MIDebuggerExited_UnknownCode {
            get {
                return ResourceManager.GetString("Error_MIDebuggerExited_UnknownCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} exited unexpectedly with exit code {1}. Debugging will now abort..
        /// </summary>
        internal static string Error_MIDebuggerExited_WithCode {
            get {
                return ResourceManager.GetString("Error_MIDebuggerExited_WithCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Required attribute &apos;{0}&apos; is missing..
        /// </summary>
        internal static string Error_MissingAttribute {
            get {
                return ResourceManager.GetString("Error_MissingAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to execute command. The MIEngine is not currently debugging any process..
        /// </summary>
        internal static string Error_NoMIDebuggerProcess {
            get {
                return ResourceManager.GetString("Error_NoMIDebuggerProcess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No terminal is available to launch the debugger.  Please install Gnome Terminal or XTerm..
        /// </summary>
        internal static string Error_NoTerminalAvailable_Linux {
            get {
                return ResourceManager.GetString("Error_NoTerminalAvailable_Linux", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commands are only accepted when the process is stopped..
        /// </summary>
        internal static string Error_ProcessMustBeStopped {
            get {
                return ResourceManager.GetString("Error_ProcessMustBeStopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &apos;{0}&apos; cannot be modified after initialization is complete..
        /// </summary>
        internal static string Error_PropertyCannotBeModifiedAfterInitialization {
            get {
                return ResourceManager.GetString("Error_PropertyCannotBeModifiedAfterInitialization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized format of field &quot;{0}&quot; in result: {1}.
        /// </summary>
        internal static string Error_ResultFormat {
            get {
                return ResourceManager.GetString("Error_ResultFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This string is null or empty..
        /// </summary>
        internal static string Error_StringIsNullOrEmpty {
            get {
                return ResourceManager.GetString("Error_StringIsNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internal error. Failed to load serializer for type &apos;{0}&apos;..
        /// </summary>
        internal static string Error_UnableToLoadSerializer {
            get {
                return ResourceManager.GetString("Error_UnableToLoadSerializer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to start debugging. {0}.
        /// </summary>
        internal static string Error_UnableToStartDebugging {
            get {
                return ResourceManager.GetString("Error_UnableToStartDebugging", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected {0} output from command &quot;{1}&quot;..
        /// </summary>
        internal static string Error_UnexpectedMIOutput {
            get {
                return ResourceManager.GetString("Error_UnexpectedMIOutput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown or unsupported target architecture &apos;{0}&apos;..
        /// </summary>
        internal static string Error_UnknownTargetArchitecture {
            get {
                return ResourceManager.GetString("Error_UnknownTargetArchitecture", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unrecognized XML element &apos;{0}&apos;..
        /// </summary>
        internal static string Error_UnknownXmlElement {
            get {
                return ResourceManager.GetString("Error_UnknownXmlElement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module containing this breakpoint has not yet loaded or the breakpoint address could not be obtained..
        /// </summary>
        internal static string Status_BreakpointPending {
            get {
                return ResourceManager.GetString("Status_BreakpointPending", resourceCulture);
            }
        }
    }
}
