using System;
using System.Threading.Tasks;
using BrightScript.Debugger.Interfaces;

namespace BrightScript.Debugger.Models
{
    public class AsyncOperation : IOperation
    {
        private Func<Task> _operation;

        public AsyncOperation(Func<Task> operation)
        {
            _operation = operation;
        }

        public Task Run()
        {
            return _operation();
        }
    }
}