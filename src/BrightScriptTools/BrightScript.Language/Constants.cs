namespace BrightScript.Language
{
    internal sealed class Constants
    {
        internal const int UIUpdateDelay = 875;
        internal const int MaximumErrorsPerFile = 25;
        internal sealed class Language
        {
            internal const string ContentType = "BrightScript";
            internal const string FileExtension = ".brs";
        }
        internal sealed class Formatting
        {
            internal const string Category = "Formatting";

            internal sealed class Pages
            {
                internal const string General = "General";
                internal const string Spacing = "Spacing";
                internal const string Indentation = "Indentation";
                internal const string WrappingAndNewLines = "WrappingNewLines";
            }
        }
    }
}