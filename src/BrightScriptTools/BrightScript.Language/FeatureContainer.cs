using BrightScript.Language.Formatting;

namespace BrightScript.Language
{
    /// <summary>
    /// Contains all the Lua language service features
    /// </summary>
    public sealed class FeatureContainer
    {
        public FeatureContainer()
        {
        }

        
        /// <summary>
        /// Gets the Formatter object
        /// </summary>
        /// <value>The Formatter</value>
        public Formatter Formatter { get; }
    }
}