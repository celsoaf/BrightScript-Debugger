using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Engine
{
    internal class WorkerThread : IWorkerThread
    {
        private ConcurrentQueue<IOperation> _operations = new ConcurrentQueue<IOperation>();

        private Thread _thread;
        private volatile bool _isClosed;

        public WorkerThread()
        {
            _thread = new Thread(Run);
            _thread.Name = "MIDebugger.PollThread";
            _thread.Start();
        }

        public event EventHandler<Exception> PostedOperationErrorEvent;

        public void Close()
        {
            _isClosed = true;
        }

        public void RunOperation(Func<Task> op)
        {
            var so = new SyncOperation(op);
            _operations.Enqueue(so);

            so.Wait();
        }

        public void PostOperation(Func<Task> op)
        {
            var ao = new AsyncOperation(op);
            _operations.Enqueue(ao);
        }

        private async void Run()
        {
            while (!_isClosed)
            {
                IOperation wm;
                if (_operations.TryDequeue(out wm))
                {
                    await wm.Run();
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public bool IsPollThread()
        {
            return Thread.CurrentThread == _thread;
        }
    }
}