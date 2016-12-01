using System;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;

namespace BrightScript.Debugger.Models
{
    public class SyncOperation : IOperation
    {
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private Func<Task> _operation;

        public SyncOperation(Func<Task> op)
        {
            _operation = op;
        }

        public Task Run()
        {
            var res = _operation();
            res.ContinueWith(t => _waitHandle.Set());
            return res;
        }

        public void Wait()
        {
            _waitHandle.WaitOne();
        }
    }
}