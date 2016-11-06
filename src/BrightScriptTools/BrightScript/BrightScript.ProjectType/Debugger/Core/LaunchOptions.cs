using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BrightScript.Debugger.Core.CommandFactories;
using Microsoft.Build.Utilities;

namespace BrightScript.Debugger.Core
{
    public enum LaunchCompleteCommand
    {
        /// <summary>
        /// Execute the 'exec-run' MI command which will spawn a new process and begin executing it.
        /// This is the default value.
        /// </summary>
        ExecRun,

        /// <summary>
        /// Execute the 'exec-continue' MI command which will resume from stopped state. This is useful if
        /// the result of setting up the debugger is that the debuggee is in break state.
        /// </summary>
        ExecContinue,

        /// <summary>
        /// No command should be executed. This is useful if the target is already ready to go.
        /// </summary>
        None,
    };

    /// <summary>
    /// Interface implemented by the android launcher. In the future we will truely make use of this as a COM
    /// interface when we are no longer using GDB. For now, we don't actually use this as a COM interface but
    /// rather as a managed interface
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("74977D02-627B-4580-BEF7-B79B8D9009EF")]
    public interface IPlatformAppLauncher : IDisposable
    {
        /// <summary>
        /// Initialized the device app launcher
        /// </summary>
        /// <param name="configStore">Current VS registry root</param>
        /// <param name="eventCallback">[Required] Callback object used to send events to the rest of Visual Studio</param>
        void Initialize(IDeviceAppLauncherEventCallback eventCallback);

        /// <summary>
        /// Initializes the launcher from the launch settings
        /// </summary>
        /// <param name="exePath">[Required] Path to the executable provided in the VsDebugTargetInfo by the project system. Some launchers may ignore this.</param>
        /// <param name="args">[Optional] Arguments to the executable provided in the VsDebugTargetInfo by the project system. Some launchers may ignore this.</param>
        /// <param name="dir">[Optional] Working directory of the executable provided in the VsDebugTargetInfo by the project system. Some launchers may ignore this.</param>
        /// <param name="launcherXmlOptions">[Required] Deserialized XML options structure</param>
        /// <param name="targetEngine">Indicates the type of debugging being done.</param>
        void SetLaunchOptions(string exePath, string args, string dir, object launcherXmlOptions);

        /// <summary>
        /// Does whatever steps are necessary to setup for debugging. On Android this will include launching
        /// the app and launching GDB server.
        /// </summary>
        /// <param name="debuggerLaunchOptions">[Required] settings to use when launching the debugger</param>
        void SetupForDebugging(out LaunchOptions debuggerLaunchOptions);

        /// <summary>
        /// Allows the device app launcher to preform any final tasks after the debugger has connected. On Android
        /// this is when we will connect to the process using JDbg.
        /// </summary>
        void OnResume();

        /// <summary>
        /// Called when terminating the application on stop debugging
        /// </summary>
        void Terminate();
    };

    /// <summary>
    /// Call back implemented by the caller of OnResume to provide a channel for errors
    /// </summary>
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6FC53A91-CB60-47E7-979B-65B7C894E794")]
    public interface IDeviceAppLauncherEventCallback
    {
        /// <summary>
        /// Call back when an error happens that should be reported to the user
        /// </summary>
        /// <param name="message">[Required] message to send</param>
        void OnWarning(string message);

        /// <summary>
        /// Used to send a custom debug event to a VS IDE service
        /// </summary>
        /// <param name="guidVSService">VS IDE service to send the event to</param>
        /// <param name="sourceId">Guid to uniquely identify the type of message</param>
        /// <param name="messageCode">Identifies the type of custom event being sent. Partners are free to define any
        /// set of values.</param>
        /// <param name="parameter1">[Optional] Specifies additional message-specific information.</param>
        /// <param name="parameter2">[Optional] Specifies additional message-specific information.</param>
        void OnCustomDebugEvent(Guid guidVSService, Guid sourceId, int messageCode, object parameter1, object parameter2);
    }

    /// <summary>
    /// Base launch options class
    /// </summary>
    public abstract class LaunchOptions
    {
        private const string XmlNamespace = "http://schemas.microsoft.com/vstudio/MDDDebuggerOptions/2014";
        private static Lazy<Assembly> s_serializationAssembly = new Lazy<Assembly>(LoadSerializationAssembly, LazyThreadSafetyMode.ExecutionAndPublication);
        private bool _initializationComplete;

        /// <summary>
        /// [Optional] Launcher used to start the application on the device
        /// </summary>
        public IPlatformAppLauncher DeviceAppLauncher { get; private set; }

        private string _exePath;
        /// <summary>
        /// [Required] Path to the executable file. This could be a path on the remote machine (for Pipe transport)
        /// or the local machine (Local transport).
        /// </summary>
        public virtual string ExePath
        {
            get { return _exePath; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("ExePath");
                VerifyCanModifyProperty("ExePath");

                _exePath = value;
            }
        }

        private string _exeArguments;
        /// <summary>
        /// [Optional] Additional arguments to specify when launching the process
        /// </summary>
        public string ExeArguments
        {
            get { return _exeArguments; }
            set
            {
                VerifyCanModifyProperty("ExeArguments");
                _exeArguments = value;
            }
        }

        private string _workingDirectory;
        /// <summary>
        /// [Optional] Working directory to use for the MI Debugger when launching the process
        /// </summary>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set
            {
                VerifyCanModifyProperty("WorkingDirectory");
                _workingDirectory = value;
            }
        }

        private string _absolutePrefixSoLibSearchPath;
        /// <summary>
        /// [Optional] Absolute prefix for directories to search for shared library symbols
        /// </summary>
        public string AbsolutePrefixSOLibSearchPath
        {
            get { return _absolutePrefixSoLibSearchPath; }
            set
            {
                VerifyCanModifyProperty("AbsolutePrefixSOLibSearchPath");
                _absolutePrefixSoLibSearchPath = value;
            }
        }

        private string _additionalSOLibSearchPath;
        /// <summary>
        /// [Optional] Additional directories to search for shared library symbols
        /// </summary>
        public string AdditionalSOLibSearchPath
        {
            get { return _additionalSOLibSearchPath; }
            set
            {
                VerifyCanModifyProperty("AdditionalSOLibSearchPath");
                _additionalSOLibSearchPath = value;
            }
        }

        private string _visualizerFile;
        /// <summary>
        /// [Optional] Natvis file name - from install location
        /// </summary>
        public string VisualizerFile
        {
            get { return _visualizerFile; }
            set
            {
                VerifyCanModifyProperty("VisualizerFile");
                _visualizerFile = value;
            }
        }

        private bool _waitDynamicLibLoad = true;
        /// <summary>
        /// If true, wait for dynamic library load to finish.
        /// </summary>
        public bool WaitDynamicLibLoad
        {
            get { return _waitDynamicLibLoad; }
            set
            {
                VerifyCanModifyProperty("WaitDynamicLibLoad");
                _waitDynamicLibLoad = value;
            }
        }

        /// <summary>
        /// If true, instead of showing Natvis-DisplayString value as a child of a dummy element, it is shown immediately.
        /// Should only be enabled if debugger is fast enough providing the value.
        /// </summary>
        public bool ShowDisplayString { get; set; }

        private ReadOnlyCollection<LaunchCommand> _setupCommands;

        /// <summary>
        /// [Required] Additional commands used to setup debugging. May be an empty collection
        /// </summary>
        public ReadOnlyCollection<LaunchCommand> SetupCommands
        {
            get { return _setupCommands; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("SetupCommands");

                VerifyCanModifyProperty("SetupCommands");
                _setupCommands = value;
            }
        }


        private ReadOnlyCollection<LaunchCommand> _customLaunchSetupCommands;

        /// <summary>
        /// [Optional] If provided, this replaces the default commands used to launch a target with some other commands. For example,
        /// this can be '-target-attach' in order to attach to a target process.An empty command list replaces the launch commands with nothing,
        /// which can be useful if the debugger is being provided launch options as command line options.
        /// </summary>
        public ReadOnlyCollection<LaunchCommand> CustomLaunchSetupCommands
        {
            get { return _customLaunchSetupCommands; }
            set
            {
                VerifyCanModifyProperty("CustomLaunchSetupCommands");
                _customLaunchSetupCommands = value;
            }
        }

        private LaunchCompleteCommand _launchCompleteCommand = LaunchCompleteCommand.None;

        public LaunchCompleteCommand LaunchCompleteCommand
        {
            get { return _launchCompleteCommand; }
            set
            {
                VerifyCanModifyProperty("LaunchCompleteCommand");
                _launchCompleteCommand = value;
            }
        }

        public static LaunchOptions GetInstance(string exePath, string args, string dir, string hostName, int port, Logger logger)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentNullException("exePath");

            logger?.WriteLine("LaunchOptions {0}:{1}", hostName, port);

            LaunchOptions launchOptions = null;
            object launcherXmlOptions = null;

            try
            {
                launchOptions = new TcpLaunchOptions(hostName, port);
            }
            catch (XmlException e)
            {
                throw new InvalidLaunchOptionsException(e.Message);
            }

            if (launchOptions.ExePath == null)
                launchOptions.ExePath = exePath;

            if (string.IsNullOrEmpty(launchOptions.ExeArguments))
                launchOptions.ExeArguments = args;

            if (string.IsNullOrEmpty(launchOptions.WorkingDirectory))
                launchOptions.WorkingDirectory = dir;

            if (launchOptions._setupCommands == null)
                launchOptions._setupCommands = new List<LaunchCommand>(capacity: 0).AsReadOnly();

            launchOptions._initializationComplete = true;
            return launchOptions;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Xml", "CA3053: UseSecureXmlResolver.",
            Justification = "Usage is secure -- XmlResolver property is set to 'null' in desktop CLR, and is always null in CoreCLR. But CodeAnalysis cannot understand the invocation since it happens through reflection.")]
        public static XmlReader OpenXml(string content)
        {
            var settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.NameTable = new NameTable();

            // set XmlResolver via reflection, if it exists. This is required for desktop CLR, as otherwise the XML reader may
            // attempt to hit untrusted external resources.
            var xmlResolverProperty = settings.GetType().GetProperty("XmlResolver", BindingFlags.Public | BindingFlags.Instance);
            xmlResolverProperty?.SetValue(settings, null);

            // Create our own namespace manager so that we can set the default namespace
            // We need this because the XML serializer requires correct namespaces,
            // but project systems may not provide it.
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(settings.NameTable);
            namespaceManager.AddNamespace(string.Empty, XmlNamespace);
            XmlParserContext context = new XmlParserContext(settings.NameTable, namespaceManager, string.Empty, XmlSpace.None);

            StringReader stringReader = null;
            XmlReader reader = null;
            bool success = false;

            try
            {
                stringReader = new StringReader(content);
                reader = XmlReader.Create(stringReader, settings, context);

                // Read to the top level element
                while (reader.NodeType != XmlNodeType.Element)
                    reader.Read();

                if (reader.NamespaceURI != XmlNamespace)
                {
                    throw new XmlException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_UnknownXmlElement, reader.Name));
                }

                success = true;
                return reader;
            }
            finally
            {
                if (!success)
                {
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                    else if (stringReader != null)
                    {
                        // NOTE: the reader will close the input, so we only want to do this
                        // if we failed to create the reader.
                        stringReader.Dispose();
                    }
                }
            }
        }

        public static object Deserialize(XmlSerializer serializer, XmlReader reader)
        {
            try
            {
                return serializer.Deserialize(reader);
            }
            catch (InvalidOperationException outerException)
            {
                // In all the cases I have seen thus far, the InvalidOperationException has a fairly useless message
                // and the inner exception message is better.
                Exception e = outerException.InnerException ?? outerException;

                throw new InvalidLaunchOptionsException(e.Message);
            }
        }

        public IEnumerable<string> GetSOLibSearchPath()
        {
            IEqualityComparer<string> comparer = StringComparer.OrdinalIgnoreCase;
            return GetSOLibSearchPathCandidates().Distinct(comparer);
        }

        /// <summary>
        /// Returns the possible paths 
        /// </summary>
        private IEnumerable<string> GetSOLibSearchPathCandidates()
        {
            char[] slashes = { '\\', '/' };

            if (_exePath != null)
            {
                // NOTE: Path.GetDirectoryName doesn't do the right thing for unix paths, so use our own logic

                int lastSlashIndex = _exePath.LastIndexOfAny(slashes);
                if (lastSlashIndex > 0)
                {
                    int exeDirectoryLength = lastSlashIndex;
                    if (exeDirectoryLength == 2 && _exePath[1] == ':')
                        exeDirectoryLength++; // for 'c:\foo.exe' we want to return 'c:\' instead of 'c:'

                    yield return _exePath.Substring(0, exeDirectoryLength);
                }
            }

            if (!string.IsNullOrEmpty(_additionalSOLibSearchPath))
            {
                foreach (string directory in _additionalSOLibSearchPath.Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(directory))
                        continue;

                    // To make sure that all directory names are in a canonical form, if there are any trailing slashes, remove them
                    string directoryWithoutTrailingSlashes = directory.TrimEnd(slashes);

                    if (directoryWithoutTrailingSlashes.Length == 2 && directoryWithoutTrailingSlashes[1] == ':')
                        yield return directoryWithoutTrailingSlashes + '\\'; // add the slash to drive letters though so the path is not relative

                    yield return directoryWithoutTrailingSlashes;
                }
            }
        }

        public static string RequireAttribute(string attributeValue, string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeValue))
                throw new InvalidLaunchOptionsException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_MissingAttribute, attributeName));

            return attributeValue;
        }

        public static int RequirePortAttribute(int attributeValue, string attributeName)
        {
            if (attributeValue <= 0 || attributeValue >= 0xffff)
            {
                throw new InvalidLaunchOptionsException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_BadRequiredAttribute, "Port"));
            }

            return attributeValue;
        }

        private static XmlSerializer GetXmlSerializer(Type type)
        {
            Assembly serializationAssembly = s_serializationAssembly.Value;
            if (serializationAssembly == null)
            {
                return new XmlSerializer(type);
            }
            else
            {
                // NOTE: You can look at MIEngine\src\MICore\obj\Debug\sgen\<random-temp-file-name>.cs to see the source code for this assembly.
                Type serializerType = serializationAssembly.GetType("Microsoft.Xml.Serialization.GeneratedAssembly." + type.Name + "Serializer");
                ConstructorInfo constructor = serializerType?.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    throw new Exception(string.Format(CultureInfo.CurrentUICulture, MICoreResources.Error_UnableToLoadSerializer, type.Name));
                }

                object serializer = constructor.Invoke(new object[0]);
                return (XmlSerializer)serializer;
            }
        }

        private static Assembly LoadSerializationAssembly()
        {
            // This code looks to see if we have sgen-created XmlSerializers assembly next to this dll, which will be true
            // when the MIEngine is running in Visual Studio. If so, it loads it, so that we can get the performance advantages
            // of a static XmlSerializers assembly. Otherwise we return null, and we will use a dynamic deserializer.

            string thisModulePath = typeof(LaunchOptions).GetTypeInfo().Assembly.ManifestModule.FullyQualifiedName;
            string thisModuleDir = Path.GetDirectoryName(thisModulePath);
            string thisModuleName = Path.GetFileNameWithoutExtension(thisModulePath);
            string serializerAssemblyPath = Path.Combine(thisModuleDir, thisModuleName + ".XmlSerializers.dll");
            if (!File.Exists(serializerAssemblyPath))
                return null;

            return Assembly.Load(new AssemblyName(thisModuleName + ".XmlSerializers"));
        }


        private void VerifyCanModifyProperty(string propertyName)
        {
            if (_initializationComplete)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_PropertyCannotBeModifiedAfterInitialization, propertyName));
        }
    }

    public sealed class TcpLaunchOptions : LaunchOptions
    {
        public TcpLaunchOptions(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentException("hostname");
            }
            if (port <= 0)
            {
                throw new ArgumentException("port");
            }

            this.Hostname = hostname;
            this.Port = port;
            this.ServerCertificateValidationCallback = null;
        }

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
    }
}