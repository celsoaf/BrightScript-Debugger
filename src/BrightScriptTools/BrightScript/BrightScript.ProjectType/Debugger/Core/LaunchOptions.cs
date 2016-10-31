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
    public enum TargetArchitecture
    {
        Unknown,
        ARM,
        ARM64,
        X86,
        X64,
        Mips
    };

    public enum TargetEngine
    {
        Unknown,
        Native,
        Java,
    }

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
        void SetLaunchOptions(string exePath, string args, string dir, object launcherXmlOptions, TargetEngine targetEngine);

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

        public MIMode DebuggerMIMode { get; set; }

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

        private TargetArchitecture _targetArchitecture;
        public TargetArchitecture TargetArchitecture
        {
            get { return _targetArchitecture; }
            set
            {
                VerifyCanModifyProperty("TargetArchitecture");
                _targetArchitecture = value;
            }
        }

        /// <summary>
        /// True if we assume that various symbol paths are going to be processed on a Unix machine
        /// </summary>
        public bool UseUnixSymbolPaths
        {
            get
            {
                if (this is LocalLaunchOptions)
                {
                    return !PlatformUtilities.IsWindows();
                }
                else
                {
                    // For now lets assume the debugger is on Unix if we are using Pipe/Tcp launch options
                    return true;
                }
            }
        }

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

        private LaunchCompleteCommand _launchCompleteCommand;

        public LaunchCompleteCommand LaunchCompleteCommand
        {
            get { return _launchCompleteCommand; }
            set
            {
                VerifyCanModifyProperty("LaunchCompleteCommand");
                _launchCompleteCommand = value;
            }
        }

        public static LaunchOptions GetInstance(string exePath, string args, string dir, string options, IDeviceAppLauncherEventCallback eventCallback, TargetEngine targetEngine, Logger logger)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentNullException("exePath");

            if (string.IsNullOrWhiteSpace(options))
                throw new InvalidLaunchOptionsException(MICoreResources.Error_StringIsNullOrEmpty);

            logger?.WriteTextBlock("LaunchOptions", options);

            LaunchOptions launchOptions = null;
            Guid clsidLauncher = Guid.Empty;
            object launcherXmlOptions = null;

            try
            {
                XmlSerializer serializer;
                using (XmlReader reader = OpenXml(options))
                {
                    switch (reader.LocalName)
                    {
                        case "LocalLaunchOptions":
                            {
                                serializer = GetXmlSerializer(typeof(MICore.Xml.LaunchOptions.LocalLaunchOptions));
                                var xmlLaunchOptions = (MICore.Xml.LaunchOptions.LocalLaunchOptions)Deserialize(serializer, reader);
                                launchOptions = LocalLaunchOptions.CreateFromXml(xmlLaunchOptions);
                            }
                            break;

                        case "PipeLaunchOptions":
                            {
                                serializer = GetXmlSerializer(typeof(MICore.Xml.LaunchOptions.PipeLaunchOptions));
                                var xmlLaunchOptions = (MICore.Xml.LaunchOptions.PipeLaunchOptions)Deserialize(serializer, reader);
                                launchOptions = PipeLaunchOptions.CreateFromXml(xmlLaunchOptions);
                            }
                            break;

                        case "TcpLaunchOptions":
                            {
                                serializer = GetXmlSerializer(typeof(MICore.Xml.LaunchOptions.TcpLaunchOptions));
                                var xmlLaunchOptions = (MICore.Xml.LaunchOptions.TcpLaunchOptions)Deserialize(serializer, reader);
                                launchOptions = TcpLaunchOptions.CreateFromXml(xmlLaunchOptions);
                            }
                            break;

                        case "IOSLaunchOptions":
                            {
                                serializer = GetXmlSerializer(typeof(MICore.Xml.LaunchOptions.IOSLaunchOptions));
                                launcherXmlOptions = Deserialize(serializer, reader);
                                clsidLauncher = new Guid("316783D1-1824-4847-B3D3-FB048960EDCF");
                            }
                            break;

                        case "AndroidLaunchOptions":
                            {
                                serializer = GetXmlSerializer(typeof(MICore.Xml.LaunchOptions.AndroidLaunchOptions));
                                launcherXmlOptions = Deserialize(serializer, reader);
                                clsidLauncher = new Guid("C9A403DA-D3AA-4632-A572-E81FF6301E9B");
                            }
                            break;

                        default:
                            {
                                throw new XmlException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_UnknownXmlElement, reader.LocalName));
                            }
                    }

                    // Read any remaining bits of XML to catch other errors
                    while (reader.NodeType != XmlNodeType.None)
                        reader.Read();
                }
            }
            catch (XmlException e)
            {
                throw new InvalidLaunchOptionsException(e.Message);
            }

            if (targetEngine == TargetEngine.Native)
            {
                if (launchOptions.ExePath == null)
                    launchOptions.ExePath = exePath;
            }

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
            IEqualityComparer<string> comparer = this.UseUnixSymbolPaths ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
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

        protected void InitializeCommonOptions(MICore.Xml.LaunchOptions.BaseLaunchOptions source)
        {
            if (this.ExePath == null)
            {
                string exePath = source.ExePath;
                if (!string.IsNullOrWhiteSpace(exePath))
                {
                    this.ExePath = exePath;
                }
            }

            if (this.TargetArchitecture == TargetArchitecture.Unknown)
            {
                this.TargetArchitecture = ConvertTargetArchitectureAttribute(source.TargetArchitecture);
            }

            Debug.Assert((uint)MIMode.Gdb == (uint)MICore.Xml.LaunchOptions.MIMode.gdb, "Enum values don't line up!");
            Debug.Assert((uint)MIMode.Lldb == (uint)MICore.Xml.LaunchOptions.MIMode.lldb, "Enum values don't line up!");
            Debug.Assert((uint)MIMode.Clrdbg == (uint)MICore.Xml.LaunchOptions.MIMode.clrdbg, "Enum values don't line up!");
            this.DebuggerMIMode = (MIMode)source.MIMode;

            if (string.IsNullOrEmpty(this.ExeArguments))
                this.ExeArguments = source.ExeArguments;

            if (string.IsNullOrEmpty(this.WorkingDirectory))
                this.WorkingDirectory = source.WorkingDirectory;

            if (string.IsNullOrEmpty(this.VisualizerFile))
                this.VisualizerFile = source.VisualizerFile;

            this.ShowDisplayString = source.ShowDisplayString;
            this.WaitDynamicLibLoad = source.WaitDynamicLibLoad;

            this.SetupCommands = LaunchCommand.CreateCollectionFromXml(source.SetupCommands);

            if (source.CustomLaunchSetupCommands != null)
            {
                this.CustomLaunchSetupCommands = LaunchCommand.CreateCollectionFromXml(source.CustomLaunchSetupCommands);
            }

            Debug.Assert((uint)LaunchCompleteCommand.ExecRun == (uint)MICore.Xml.LaunchOptions.BaseLaunchOptionsLaunchCompleteCommand.execrun);
            Debug.Assert((uint)LaunchCompleteCommand.ExecContinue == (uint)MICore.Xml.LaunchOptions.BaseLaunchOptionsLaunchCompleteCommand.execcontinue);
            Debug.Assert((uint)LaunchCompleteCommand.None == (uint)MICore.Xml.LaunchOptions.BaseLaunchOptionsLaunchCompleteCommand.None);
            this.LaunchCompleteCommand = (LaunchCompleteCommand)source.LaunchCompleteCommand;

            string additionalSOLibSearchPath = source.AdditionalSOLibSearchPath;
            if (!string.IsNullOrEmpty(additionalSOLibSearchPath))
            {
                if (string.IsNullOrEmpty(this.AdditionalSOLibSearchPath))
                    this.AdditionalSOLibSearchPath = additionalSOLibSearchPath;
                else
                    this.AdditionalSOLibSearchPath = string.Concat(this.AdditionalSOLibSearchPath, ";", additionalSOLibSearchPath);
            }

            if (string.IsNullOrEmpty(this.AbsolutePrefixSOLibSearchPath))
                this.AbsolutePrefixSOLibSearchPath = source.AbsolutePrefixSOLibSearchPath;
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

        public static TargetArchitecture ConvertTargetArchitectureAttribute(MICore.Xml.LaunchOptions.TargetArchitecture source)
        {
            switch (source)
            {
                case MICore.Xml.LaunchOptions.TargetArchitecture.X86:
                case MICore.Xml.LaunchOptions.TargetArchitecture.x86:
                    return TargetArchitecture.X86;

                case MICore.Xml.LaunchOptions.TargetArchitecture.arm:
                case MICore.Xml.LaunchOptions.TargetArchitecture.ARM:
                    return TargetArchitecture.ARM;

                case MICore.Xml.LaunchOptions.TargetArchitecture.mips:
                case MICore.Xml.LaunchOptions.TargetArchitecture.MIPS:
                    return TargetArchitecture.Mips;

                case MICore.Xml.LaunchOptions.TargetArchitecture.x64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.amd64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.x86_64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.X64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.AMD64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.X86_64:
                    return TargetArchitecture.X64;

                case MICore.Xml.LaunchOptions.TargetArchitecture.arm64:
                case MICore.Xml.LaunchOptions.TargetArchitecture.ARM64:
                    return TargetArchitecture.ARM64;

                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_UnknownTargetArchitecture, source.ToString()));
            }
        }
    }

    public sealed class EnvironmentEntry
    {
        public EnvironmentEntry(MICore.Xml.LaunchOptions.EnvironmentEntry xmlEntry)
        {
            this.Name = xmlEntry.Name;
            this.Value = xmlEntry.Value;
        }

        /// <summary>
        /// [Required] Name of the environment variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// [Required] Value of the environment variable
        /// </summary>
        public string Value { get; private set; }
    }

    public sealed class LocalLaunchOptions : LaunchOptions
    {
        private string _coreDumpPath;
        private bool _useExternalConsole;

        private const int DefaultLaunchTimeout = 10 * 1000; // 10 seconds

        public LocalLaunchOptions(string MIDebuggerPath, string MIDebuggerServerAddress, int processId, MICore.Xml.LaunchOptions.EnvironmentEntry[] environmentEntries)
        {
            if (string.IsNullOrEmpty(MIDebuggerPath))
                throw new ArgumentNullException("MIDebuggerPath");

            this.MIDebuggerPath = MIDebuggerPath;
            this.MIDebuggerServerAddress = MIDebuggerServerAddress;
            this.ProcessId = processId;

            List<EnvironmentEntry> environmentList = new List<EnvironmentEntry>();
            if (environmentEntries != null)
            {
                foreach (MICore.Xml.LaunchOptions.EnvironmentEntry xmlEntry in environmentEntries)
                {
                    environmentList.Add(new EnvironmentEntry(xmlEntry));
                }
            }

            this.Environment = new ReadOnlyCollection<EnvironmentEntry>(environmentList);
        }

        private void InitializeServerOptions(MICore.Xml.LaunchOptions.LocalLaunchOptions source)
        {
            if (!String.IsNullOrWhiteSpace(source.DebugServer))
            {
                DebugServer = source.DebugServer;
                DebugServerArgs = source.DebugServerArgs;
                ServerStarted = source.ServerStarted;
                FilterStderr = source.FilterStderr;
                FilterStdout = source.FilterStdout;
                if (!FilterStderr && !FilterStdout)
                {
                    FilterStdout = true;    // no pattern source specified, use stdout
                }
                ServerLaunchTimeout = source.ServerLaunchTimeoutSpecified ? source.ServerLaunchTimeout : DefaultLaunchTimeout;
            }
        }

        /// <summary>
        /// Checks that the path is valid, exists, and is rooted.
        /// </summary>
        private static bool CheckPath(string path)
        {
            return path.IndexOfAny(Path.GetInvalidPathChars()) < 0 && File.Exists(path) && Path.IsPathRooted(path);
        }

        /// <summary>
        /// Checks that if the directory path is valid, exists and is rooted.
        /// </summary>
        public static bool CheckDirectoryPath(string path)
        {
            return path.IndexOfAny(Path.GetInvalidPathChars()) < 0 && Directory.Exists(path) && Path.IsPathRooted(path);
        }

        public bool IsCoreDump
        {
            get { return !String.IsNullOrEmpty(this.CoreDumpPath); }
        }

        public bool ShouldStartServer()
        {
            return !string.IsNullOrWhiteSpace(DebugServer);
        }

        public bool IsValidMiDebuggerPath()
        {
            return File.Exists(MIDebuggerPath);
        }

        static internal LocalLaunchOptions CreateFromXml(MICore.Xml.LaunchOptions.LocalLaunchOptions source)
        {
            var options = new LocalLaunchOptions(
                RequireAttribute(source.MIDebuggerPath, "MIDebuggerPath"),
                source.MIDebuggerServerAddress,
                source.ProcessId,
                source.Environment);
            options.InitializeCommonOptions(source);
            options.InitializeServerOptions(source);
            options.CoreDumpPath = source.CoreDumpPath;
            options._useExternalConsole = source.ExternalConsole;

            // Ensure that CoreDumpPath and ProcessId are not specified at the same time
            if (!String.IsNullOrEmpty(source.CoreDumpPath) && source.ProcessId != 0)
                throw new InvalidLaunchOptionsException(String.Format(CultureInfo.InvariantCulture, MICoreResources.Error_CannotSpecifyBoth, nameof(source.CoreDumpPath), nameof(source.ProcessId)));

            return options;
        }

        /// <summary>
        /// [Required] Path to the MI Debugger Executable.
        /// </summary>
        public string MIDebuggerPath { get; private set; }

        /// <summary>
        /// [Optional] Server address that MI Debugger server is listening to
        /// </summary>
        public string MIDebuggerServerAddress { get; private set; }

        /// <summary>
        /// [Optional] If supplied, the debugger will attach to the process rather than launching a new one. Note that some operating systems will require admin rights to do this.
        /// </summary>
        public int ProcessId { get; private set; }

        /// <summary>
        /// [Optional] List of environment variables to add to the launched process
        /// </summary>
        public ReadOnlyCollection<EnvironmentEntry> Environment { get; private set; }

        /// <summary>
        /// [Optional] MI Debugger Server exe, if non-null then the MIEngine will start the debug server before starting the debugger
        /// </summary>
        public string DebugServer { get; private set; }

        /// <summary>
        /// [Optional] Args for MI Debugger Server exe
        /// </summary>
        public string DebugServerArgs { get; private set; }

        /// <summary>
        /// [Optional] Server started pattern (in Regex format)
        /// </summary>
        public string ServerStarted { get; private set; }

        /// <summary>
        /// [Optional] Log strings written to stderr and examine for server started pattern
        /// </summary>
        public bool FilterStderr { get; private set; }

        /// <summary>
        /// [Optional] Log strings written to stdout and examine for server started pattern
        /// </summary>
        public bool FilterStdout { get; private set; }

        /// <summary>
        /// [Optional] Log strings written to stderr and examine for server started pattern
        /// </summary>
        public int ServerLaunchTimeout { get; private set; }

        /// <summary>
        /// [Optional] Path to a core dump file for the specified executable.
        /// </summary>
        public string CoreDumpPath
        {
            get
            {
                return _coreDumpPath;
            }
            private set
            {
                // CoreDumpPath is allowed to be null/empty
                if (!String.IsNullOrEmpty(value) && !LocalLaunchOptions.CheckPath(value))
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, MICoreResources.Error_InvalidLocalExePath, value));

                _coreDumpPath = value;
            }
        }

        public bool UseExternalConsole
        {
            get { return _useExternalConsole; }
        }
    }

    /// <summary>
    /// Launch options when connecting to an instance of an MI Debugger running on a remote device through a shell
    /// </summary>
    public sealed class PipeLaunchOptions : LaunchOptions
    {
        /// <summary>
        /// Creates an instance of PipeLaunchOptions
        /// </summary>
        /// <param name="pipePath">Path of the pipe program</param>
        /// <param name="pipeArguments">Argument to the pipe program</param>
        /// <param name="pipeCommandArguments">Command to be invoked on the pipe program</param>
        /// <param name="pipeCwd">Current working directory of pipe program. If empty directory of the pipePath is set as the cwd.</param>
        /// <param name="pipeEnvironment">Environment variables set before invoking the pipe program</param>
        public PipeLaunchOptions(string pipePath, string pipeArguments, string pipeCommandArguments, string pipeCwd, MICore.Xml.LaunchOptions.EnvironmentEntry[] pipeEnvironment)
        {
            if (string.IsNullOrEmpty(pipePath))
                throw new ArgumentNullException("PipePath");

            this.PipePath = pipePath;
            this.PipeArguments = pipeArguments;
            this.PipeCommandArguments = pipeCommandArguments;
            this.PipeCwd = pipeCwd;

            this.PipeEnvironment = (pipeEnvironment != null) ? pipeEnvironment.Select(e => new EnvironmentEntry(e)).ToArray() : new EnvironmentEntry[] { };
        }

        static internal PipeLaunchOptions CreateFromXml(MICore.Xml.LaunchOptions.PipeLaunchOptions source)
        {
            var options = new PipeLaunchOptions(RequireAttribute(source.PipePath, "PipePath"), source.PipeArguments, source.PipeCommandArguments, source.PipeCwd, source.PipeEnvironment);
            options.InitializeCommonOptions(source);

            return options;
        }

        /// <summary>
        /// [Required] Path to the pipe executable.
        /// </summary>
        public string PipePath { get; private set; }

        /// <summary>
        /// [Optional] Arguments to pass to the pipe executable.
        /// </summary>
        /// 
        public string PipeArguments { get; private set; }

        /// <summary>
        /// [Optional] Arguments to pass to the PipePath program that include a format specifier ('{0}') for a custom command.
        /// </summary>
        public string PipeCommandArguments { get; private set; }

        /// <summary>
        /// [Optional] Current working directory when the pipe program is invoked.
        /// </summary>
        public string PipeCwd { get; private set; }

        /// <summary>
        /// [Optional] Enviroment variables for the pipe program.
        /// </summary>
        public IReadOnlyCollection<EnvironmentEntry> PipeEnvironment { get; private set; }
    }

    public sealed class TcpLaunchOptions : LaunchOptions
    {
        public TcpLaunchOptions(string hostname, int port, bool secure)
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
            this.Secure = secure;
            this.ServerCertificateValidationCallback = null;
        }

        static internal TcpLaunchOptions CreateFromXml(MICore.Xml.LaunchOptions.TcpLaunchOptions source)
        {
            var options = new TcpLaunchOptions(RequireAttribute(source.Hostname, "Hostname"), LaunchOptions.RequirePortAttribute(source.Port, "Port"), source.Secure);
            options.InitializeCommonOptions(source);

            return options;
        }

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public bool Secure { get; private set; }
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
    }
}