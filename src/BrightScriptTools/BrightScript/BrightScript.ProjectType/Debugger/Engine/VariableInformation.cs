using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class VariableInformation : IVariableInformation
    {
        internal VariableInformation(string expr, VariableInformation parent)
        {
            
        }


        internal VariableInformation(string expr, ThreadContext ctx, AD7Engine engine, AD7Thread thread, bool isParameter = false)
        {
            
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public string Name { get; }
        public string Value { get; }
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
            throw new System.NotImplementedException();
        }

        public bool IsStringType { get; }
        public void EnsureChildren()
        {
            throw new System.NotImplementedException();
        }

        public void AsyncEval(IDebugEventCallback2 pExprCallback)
        {
            throw new System.NotImplementedException();
        }

        public void AsyncError(IDebugEventCallback2 pExprCallback, IDebugProperty2 error)
        {
            throw new System.NotImplementedException();
        }

        public void SyncEval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            throw new System.NotImplementedException();
        }

        public ThreadContext ThreadContext { get; }
        public IVariableInformation FindChildByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public string EvalDependentExpression(string expr)
        {
            throw new System.NotImplementedException();
        }

        public bool IsVisualized { get; }
        public enum_DEBUGPROP_INFO_FLAGS PropertyInfoFlags { get; set; }
        public void Assign(string expression)
        {
            throw new System.NotImplementedException();
        }

        public Task Eval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            throw new System.NotImplementedException();
        }
    }
}