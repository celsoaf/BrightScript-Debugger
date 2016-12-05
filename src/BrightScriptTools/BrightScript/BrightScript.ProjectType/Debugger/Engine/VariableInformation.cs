using System;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class VariableInformation : IVariableInformation
    {
        private ThreadContext _ctx;
        private AD7Engine _engine;
        private AD7Thread _thread;
        private readonly bool _isParameter;
        private readonly VariableInformation _parent;

        internal VariableInformation(string expr, VariableInformation parent)
        {
            ProcessExp(expr);
            _parent = parent;
        }

        private void ProcessExp(string expr)
        {
            if (expr.Contains("="))
            {
                var parts = expr.Split('=');
                Name = parts[0];
                Value = parts[1];
            }
            else
            {
                Name = expr;
            }
        }


        internal VariableInformation(string expr, ThreadContext ctx, AD7Engine engine, AD7Thread thread, bool isParameter = false)
        {
            ProcessExp(expr);
            _ctx = ctx;
            _engine = engine;
            _thread = thread;
            _isParameter = isParameter;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
        public string TypeName { get; }
        public bool IsParameter { get; }
        public IVariableInformation[] Children { get; }
        public AD7Thread Client { get; }
        public bool Error { get; }
        public uint CountChildren { get; }
        public bool IsChild { get; set; }
        public enum_DBG_ATTRIB_FLAGS Access { get; }

        public string FullName()
        {
            return Name;
        }

        public bool IsStringType { get; }
        public void EnsureChildren()
        {
            //throw new System.NotImplementedException();
            Console.WriteLine();
        }

        public void AsyncEval(IDebugEventCallback2 pExprCallback)
        {
            //throw new System.NotImplementedException();
            Console.WriteLine();
        }

        public void AsyncError(IDebugEventCallback2 pExprCallback, IDebugProperty2 error)
        {
            //throw new System.NotImplementedException();
            Console.WriteLine();
        }

        public void SyncEval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            //throw new System.NotImplementedException();
            Console.WriteLine();
        }

        public ThreadContext ThreadContext { get; }
        public IVariableInformation FindChildByName(string name)
        {
            //throw new System.NotImplementedException();
            return null;
        }

        public string EvalDependentExpression(string expr)
        {
            //throw new System.NotImplementedException();
            return null;
        }

        public bool IsVisualized { get; }
        public enum_DEBUGPROP_INFO_FLAGS PropertyInfoFlags { get; set; }
        public void Assign(string expression)
        {
            //throw new System.NotImplementedException();
            Console.WriteLine();
        }

        public async Task Eval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            Value = await _engine.DebuggedProcess.CommandFactory.Print(Name);
        }
    }
}