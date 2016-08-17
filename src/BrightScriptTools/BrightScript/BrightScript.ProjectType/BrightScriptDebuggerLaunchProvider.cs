using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Debuggers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.DebuggerProviders;
using Microsoft.VisualStudio.ProjectSystem.VS.Debuggers;

namespace BrightScript
{
    [ExportDebugger(BrightScriptDebugger.SchemaName)]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    public class BrightScriptDebuggerLaunchProvider : DebugLaunchProviderBase
    {
        // TODO: Specify the assembly full name here
        [ExportPropertyXamlRuleDefinition("BrightScript, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9be6e469bc4921f1", "XamlRuleToCode:BrightScriptDebugger.xaml", "Project")]
        [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
        private object DebuggerXaml { get { throw new NotImplementedException(); } }

        [ImportingConstructor]
        public BrightScriptDebuggerLaunchProvider(ConfiguredProject configuredProject)
            : base(configuredProject)
        {
        }

        /// <summary>
        /// Gets project properties that the debugger needs to launch.
        /// </summary>
        [Import]
        private ProjectProperties DebuggerProperties { get; set; }

        public override async Task<bool> CanLaunchAsync(DebugLaunchOptions launchOptions)
        {
            var properties = await this.DebuggerProperties.GetBrightScriptDebuggerPropertiesAsync();
            string commandValue = await properties.BrightScriptDebuggerCommand.GetEvaluatedValueAtEndAsync();
            return !string.IsNullOrEmpty(commandValue);
        }

        public override async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(DebugLaunchOptions launchOptions)
        {
            var settings = new DebugLaunchSettings(launchOptions);

            // The properties that are available via DebuggerProperties are determined by the property XAML files in your project.
            var debuggerProperties = await this.DebuggerProperties.GetBrightScriptDebuggerPropertiesAsync();
            settings.CurrentDirectory = await debuggerProperties.BrightScriptDebuggerWorkingDirectory.GetEvaluatedValueAtEndAsync();
            settings.Executable = await debuggerProperties.BrightScriptDebuggerCommand.GetEvaluatedValueAtEndAsync();
            settings.Arguments = await debuggerProperties.BrightScriptDebuggerCommandArguments.GetEvaluatedValueAtEndAsync();
            settings.LaunchOperation = DebugLaunchOperation.CreateProcess;

            // TODO: Specify the right debugger engine
            settings.LaunchDebugEngineGuid = DebuggerEngines.ManagedOnlyEngine;

            return new IDebugLaunchSettings[] { settings };
        }

        public override Task LaunchAsync(DebugLaunchOptions launchOptions)
        {
            return base.LaunchAsync(launchOptions);
        }
    }
}
