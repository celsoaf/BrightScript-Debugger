using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IThreadCache
    {
        Task<DebuggedThread[]> GetThreads();
        DebuggedThread FindThread(int id);

        Task<ThreadContext> GetThreadContext(DebuggedThread thread);
        Task<List<ThreadContext>> StackFrames(DebuggedThread thread);

        void MarkDirty();
        void SendThreadEvents();

        void SetStackFrames(int id, List<ThreadContext> frames);
    }
}