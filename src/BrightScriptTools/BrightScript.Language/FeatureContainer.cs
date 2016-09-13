using BrightScript.Language.Diagnostics;
using BrightScript.Language.Formatting;
using BrightScript.Language.Shared;

namespace BrightScript.Language
{
    /// <summary>
    /// Contains all the Lua language service features
    /// </summary>
    internal sealed class FeatureContainer
    {
        private ISingletons _singletons;

        internal FeatureContainer(ISingletons singletons)
        {
            _singletons = singletons;
            this.Formatter = new Formatter();
            this.DiagnosticsProvider = new DiagnosticsProvider(singletons);
        }

        public IDiagnosticsProvider DiagnosticsProvider { get; }


        /// <summary>
        /// Gets the Formatter object
        /// </summary>
        /// <value>The Formatter</value>
        public Formatter Formatter { get; }
    }
}