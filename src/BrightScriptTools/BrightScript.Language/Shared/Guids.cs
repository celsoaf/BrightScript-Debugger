using System;
using System.Diagnostics.CodeAnalysis;

namespace BrightScript.Language.Shared
{
    internal sealed class Guids
    {
        internal const string PackageString = "094DDC85-2F04-4A20-B362-3F4B1E1718F5";

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        internal static readonly Guid Package = new Guid(PackageString);

        internal const string ServiceString = "CD9F985F-1C37-4BDF-B83F-E8527F9C6A1B";

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
        internal static readonly Guid Service = new Guid(ServiceString);
    }
}