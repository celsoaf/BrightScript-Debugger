using System;
using System.Globalization;

namespace BrightScript.Debugger.Core
{
    public class InvalidLaunchOptionsException : Exception
    {
        internal InvalidLaunchOptionsException(string problemDescription) :
            base(string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_InvalidLaunchOptions, problemDescription))
        {
        }
    }
}