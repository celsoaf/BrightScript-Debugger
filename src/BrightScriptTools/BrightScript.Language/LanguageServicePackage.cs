using System.Runtime.InteropServices;
using BrightScript.Language.Formatting;
using BrightScript.Language.Shared;
using Microsoft.VisualStudio.Shell;

namespace BrightScript.Language
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Guid(Guids.PackageString)]
    public class LanguageServicePackage : Package
    {
        internal UserSettings FormattingUserSettings
        {
            get
            {
                return (UserSettings)this.GetAutomationObject($"{Constants.Formatting.Category}.{Constants.Formatting.Pages.General}");
            }
        }
    }
}