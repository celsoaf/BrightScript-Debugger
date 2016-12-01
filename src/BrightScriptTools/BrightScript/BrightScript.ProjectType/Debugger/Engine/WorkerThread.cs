using System;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;

namespace BrightScript.Debugger.Engine
{
    internal class WorkerThread : IWorkerThread
    {
        public event EventHandler<Exception> PostedOperationErrorEvent;

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public void RunOperation(Func<Task> op)
        {
            throw new NotImplementedException();
        }

        public void RunOperation(string text, CancellationTokenSource canTokenSource, Func<EventWaitHandle, Task> op)
        {
            throw new NotImplementedException();
        }
    }
}