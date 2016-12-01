using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IThreadCache
    {
        Task<DebuggedThread[]> GetThreads();

        Task<ThreadContext> GetThreadContext(DebuggedThread thread);
        Task<List<ThreadContext>> StackFrames(DebuggedThread thread);
    }
}