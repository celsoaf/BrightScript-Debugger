using System;
using System.Collections.Generic;
using BrightScript.Loggger;

namespace BrightScript.Debugger.Core
{

    /// <summary>
    /// Abstraction over a named section within the HostConfigurationStore. This provides 
    /// the ability to enumerate values within the section.
    /// </summary>
    public sealed class HostConfigurationSection : IDisposable
    {
        private HostConfigurationSection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases any resources held by the HostConfigurationSection.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Obtains the value of the specified valueName
        /// </summary>
        /// <param name="valueName">Name of the value to obtain</param>
        /// <returns>[Optional] null if the value doesn't exist, otherwise the value
        /// </returns>
        public object GetValue(string valueName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enumerates the names of all the values defined in this section
        /// </summary>
        /// <returns>Enumerator of strings</returns>
        public IEnumerable<string> GetValueNames()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Provides access to settings for the engine
    /// </summary>
    public sealed class HostConfigurationStore
    {
        private readonly string _registryRoot;
        private readonly string _engineId;

        /// <summary>
        /// Constructs a new HostConfigurationStore object. This API should generally be 
        /// called from an engine's implementation of IDebugEngine2.SetRegistryRoot.
        /// </summary>
        /// <param name="registryRoot">registryRoot value provided in SetRegistryRoot. 
        /// In Visual Studio, this will be something like 'Software\\Microsoft\\VisualStudio\\14.0'. 
        /// In VS Code this will not really be a registry value but rather a key used to 
        /// find the right configuration file.</param>
        /// <param name="engineId">The engine id of this engine.</param>
        public HostConfigurationStore(string registryRoot, string engineId)
        {
            _registryRoot = registryRoot;
            _engineId = engineId;
        }

        /// <summary>
        /// Provides the registry root string. This is NOT supported in VS Code, and this property may eventually be removed.
        /// </summary>
        public string RegistryRoot
        {
            get { return _registryRoot; }
        }

        /// <summary>
        /// Reads the specified engine metric.
        /// </summary>
        /// <param name="metric">The metric to read.</param>
        /// <returns>[Optional] value of the metric. Null if the metric is not defined.</returns>
        public object GetEngineMetric(string metric)
        {
            return null;
        }

        /// <summary>
        /// Obtains exception settings for the specified exception category.
        /// </summary>
        /// <param name="categoryId">The GUID used to identify the exception category</param>
        /// <param name="categoryConfigSection">The configuration section where exception values can be obtained.</param>
        /// <param name="categoryName">The name of the exception category.</param>
        public void GetExceptionCategorySettings(Guid categoryId, out HostConfigurationSection categoryConfigSection, out string categoryName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if logging is enabled, and if so returns a logger object. 
        /// 
        /// In VS, this is wired up to read from the registry and return a logger which writes a log file to %TMP%\log-file-name.
        /// In VS Code, this will check if the '--engineLogging' switch is enabled, and if so return a logger that will write to the Console.
        /// </summary>
        /// <param name="enableLoggingSettingName">[Optional] In VS, the name of the settings key to check if logging is enabled. 
        /// If not specified, this will check 'EnableLogging' in the AD7 Metrics.</param>
        /// <param name="logFileName">[Required] name of the log file to open if logging is enabled.</param>
        /// <returns>[Optional] If logging is enabled, the logging object.</returns>
        public HostLogger GetLogger(string enableLoggingSettingName, string logFileName)
        {
            return new HostLogger();
        }

        /// <summary>
        /// Read the debugger setting
        /// 
        /// In VS, this is wired up to read setting value from RegistryRoot\\Debugger\\
        /// </summary>
        /// <returns>value of the setting</returns>
        public T GetDebuggerConfigurationSetting<T>(string settingName, T defaultValue)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The host logger returned from HostConfigurationStore.GetLogger.
    /// </summary>
    public sealed class HostLogger
    {
        /// <summary>
        /// Writes a line to the log
        /// </summary>
        /// <param name="line">Line to write.</param>
        public void WriteLine(string line)
        {
            LiveLogger.WriteLine(line);
        }

        /// <summary>
        /// If the log is implemented as a file, this flushes the file.
        /// </summary>
        public void Flush()
        {
            
        }
    }
}