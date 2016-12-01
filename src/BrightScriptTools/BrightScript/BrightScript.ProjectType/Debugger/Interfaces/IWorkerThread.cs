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
        void PostOperation(Func<Task> op);
    }
}