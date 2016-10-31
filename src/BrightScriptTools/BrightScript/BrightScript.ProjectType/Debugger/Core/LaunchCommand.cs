using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BrightScript.Debugger.Core
{
    /// <summary>
    /// A {command, description, ignore failure} tuple for a launch/setup command. These are either read from an XML launch options blob, or returned from a launcher.
    /// </summary>
    public class LaunchCommand
    {
        public readonly string CommandText;
        public readonly string Description;
        public readonly bool IgnoreFailures;
        public readonly bool IsMICommand;
        public /*OPTIONAL*/ Action<string> FailureHandler { get; private set; }

        public LaunchCommand(string commandText, string description = null, bool ignoreFailures = false, Action<string> failureHandler = null)
        {
            if (commandText == null)
                throw new ArgumentNullException("commandText");
            commandText = commandText.Trim();
            if (commandText.Length == 0)
                throw new ArgumentOutOfRangeException("commandText");
            this.IsMICommand = commandText[0] == '-';
            this.CommandText = commandText;
            this.Description = description;
            if (string.IsNullOrWhiteSpace(description))
                this.Description = this.CommandText;

            this.IgnoreFailures = ignoreFailures;
            this.FailureHandler = failureHandler;
        }
    }
}