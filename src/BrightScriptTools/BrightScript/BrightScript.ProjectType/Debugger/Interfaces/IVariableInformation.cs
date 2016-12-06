using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Interfaces
{
    internal interface IVariableInformation : IDisposable
    {
        string Name { get; }
        string Value { get; }
        string TypeName { get; }
        bool IsParameter { get; }
        List<IVariableInformation> Children { get; } // children are never synthetic
        AD7Thread Client { get; }
        bool Error { get; }
        uint CountChildren { get; }
        bool IsChild { get; set; }
        enum_DBG_ATTRIB_FLAGS Access { get; }
        string FullName();
        bool IsStringType { get; }
        void EnsureChildren();
        void AsyncEval(IDebugEventCallback2 pExprCallback);
        void AsyncError(IDebugEventCallback2 pExprCallback, IDebugProperty2 error);
        void SyncEval(enum_EVALFLAGS dwFlags = 0);
        ThreadContext ThreadContext { get; }

        NodeType VariableNodeType { get; }
        IVariableInformation FindChildByName(string name);
        string EvalDependentExpression(string expr);
        bool IsVisualized { get; }
        enum_DEBUGPROP_INFO_FLAGS PropertyInfoFlags { get; set; }

        void Assign(string expression);
        Task Eval(enum_EVALFLAGS dwFlags = 0);
    }
}