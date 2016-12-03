using System;
using BrightScript.Debugger.Transport;

namespace BrightScript.Debugger.Interfaces
{
    public interface IRokuController : ITransportCallback
    {
        event Action<string> OnOutput;
        void Connect();
        void Close();
    }
}