using BrightScript.Language.Diagnostics;
using BrightScript.Language.Formatting;
using BrightScript.Language.Shared;

namespace BrightScript.Language
{
    /// <summary>
    /// Contains all the BrightScript language service features
    /// </summary>
    internal sealed class FeatureContainer
    {
        private ParseTreeCache parseTreeCache;


        internal FeatureContainer()
        {
            this.parseTreeCache = new ParseTreeCache();
            this.Formatter = new Formatter();
            this.DiagnosticsProvider = new DiagnosticsProvider(this.parseTreeCache);
        }

        public IDiagnosticsProvider DiagnosticsProvider { get; }


        /// <summary>
        /// Gets the Formatter object
        /// </summary>
        /// <value>The Formatter</value>
        public Formatter Formatter { get; }
    }
}