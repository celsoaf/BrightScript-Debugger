using System;

namespace BrightScript.Debugger.Exceptions
{
    public class MIException : Exception
    {
        public MIException(int hr)
        {
            this.HResult = hr;
        }

        public MIException(int hr, Exception innerException)
            : base(string.Empty, innerException)
        {
        }
    }
}