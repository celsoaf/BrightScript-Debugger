using System;
using System.Threading;
using System.Threading.Tasks;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IWorkerThread
    {
        event EventHandler<Exception> PostedOperationErrorEvent;

        void Close();
        void RunOperation(Func<Task> op);
        //void RunOperation(Action op);
        void RunOperation(string text, CancellationTokenSource canTokenSource, Func<EventWaitHandle, Task> op);
    }
}