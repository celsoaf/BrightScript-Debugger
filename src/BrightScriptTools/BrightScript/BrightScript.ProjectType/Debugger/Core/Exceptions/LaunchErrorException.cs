using System;

namespace BrightScript.Debugger.Engine
{
    internal class LaunchErrorException : Exception
    {
        public LaunchErrorException(string message) : base(message)
        {
        }
    }
}