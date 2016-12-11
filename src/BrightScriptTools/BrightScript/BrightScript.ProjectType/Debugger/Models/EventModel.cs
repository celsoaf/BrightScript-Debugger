using System.Threading;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Models
{
    public class EventModel
    {
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        public EventModel(IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program, IDebugThread2 thread)
        {
            EventObject = eventObject;
            IidEvent = iidEvent;
            Program = program;
            Thread = thread;
        }

        public IDebugEvent2 EventObject { get; }
        public string IidEvent { get; }
        public IDebugProgram2 Program { get; }
        public IDebugThread2 Thread { get; }

        public void Wait()
        {
            _waitHandle.WaitOne();
        }

        public void Set()
        {
            _waitHandle.Set();
        }
    }
}