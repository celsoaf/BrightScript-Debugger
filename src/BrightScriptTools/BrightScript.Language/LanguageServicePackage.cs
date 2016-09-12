using BrightScript.Language.Formatting;
using Microsoft.VisualStudio.Shell;

namespace BrightScript.Language
{
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