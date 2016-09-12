using System.Collections.Generic;
using System.Collections.Immutable;

namespace BrightScript.Language.Formatting.Options
{
    /// <summary>
    /// OptionalRuleMap holds all the Rules that can be turned off, and is sent in as a parameter
    /// when the Rules are changed in "Update" in GlobalOptions.
    /// </summary>
    internal class OptionalRuleMap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules",
             "SA1401:Fields must be private", Justification = "<Pending>")] internal readonly HashSet<DisableableRules>
            DisabledRuleGroups = new HashSet<DisableableRules>();

        /// <summary>
        /// Allows Rule disabling.
        /// </summary>
        /// <param name="optionalRuleGroups">
        /// The OptionalRuleGroups that are to be disabled/skipped.
        /// </param>
        internal OptionalRuleMap(IEnumerable<DisableableRules> optionalRuleGroups)
        {
            foreach (DisableableRules group in optionalRuleGroups)
            {
                this.Disable(group);
            }
        }

        private void Disable(DisableableRules optionalRuleGroup)
        {

        }
    }
}