using System;
using BrightScript.Debugger.Transport;

namespace BrightScript.Debugger.Interfaces
{
    public interface IRokuController : ITransportCallback
    {
        event Action<string> OnOutput;
        event Action<int> BreakModeEvent;
        event Action RunModeEvent;
        event Action ProcessExitEvent;

        void Connect();
        void Close();
    }
}