using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Transport;

namespace BrightScript.Debugger.Interfaces
{
    public interface IRokuController : ITransportCallback
    {
        event Action<string> OnOutput;
        event Action<List<ThreadContext>> OnBackTrace;

        event Action<int> BreakModeEvent;
        event Action RunModeEvent;
        event Action ProcessExitEvent;

        void Connect();
        void Close();

        Task<Res> CmdAsync<Res>(CommandModel cmd) where Res : class;
    }
}