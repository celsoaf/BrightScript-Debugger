using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class VariableInformation : IVariableInformation
    {
        private static Dictionary<string, NodeType> mapper = new Dictionary<string, NodeType>()
        {
            { "<Component: roAssociativeArray> =", NodeType.Field }   
        };

        private string _internalName;  // the MI debugger's private name for this value
        private AD7Engine _engine;
        private DebuggedProcess _debuggedProcess;
        private ThreadContext _ctx;
        private bool _attribsFetched;
        private bool _isReadonly;
        private string _format;
        private string _strippedName;  // "Name" stripped of format specifiers
        private IVariableInformation _parent;
        private string _fullname;

        public string Name { get; private set; }
        public string Value { get; private set; }
        public string TypeName { get; private set; }
        public bool IsParameter { get; private set; }
        public List<IVariableInformation> Children { get;  } = new List<IVariableInformation>();
        public AD7Thread Client { get; private set; }
        public bool Error { get; private set; }
        public uint CountChildren { get { return (uint)Children.Count; } }
        public bool IsChild { get; set; }
        public enum_DBG_ATTRIB_FLAGS Access { get; private set; }
        public bool IsVisualized { get { return _parent == null ? false : _parent.IsVisualized; } }
        public enum_DEBUGPROP_INFO_FLAGS PropertyInfoFlags { get; set; }

        public NodeType VariableNodeType { get; private set; }

        internal VariableInformation(string expr, VariableInformation parent)
        {
            Name = expr;
            _parent = parent;
            _ctx = parent._ctx;
            _engine = parent._engine;
            Client = parent.Client;
        }


        internal VariableInformation(string expr, ThreadContext ctx, AD7Engine engine, AD7Thread thread, bool isParameter = false)
        {
            Name = expr;
            _ctx = ctx;
            _engine = engine;
            Client = thread;
            IsParameter = isParameter;
        }

        public void Dispose()
        {
            
        }

        public string FullName()
        {
            if (_parent != null)
                return _parent.FullName() + "." + Name;
            return Name;
        }

        public bool IsStringType { get; }
        public void EnsureChildren()
        {
            if (CountChildren != 0)
            {
                Task task = FetchChildren();
                task.Wait();
            }
        }

        private async Task FetchChildren()
        {
            foreach (var variableInformation in Children)
            {
                await variableInformation.Eval();
            }   
        }

        public void AsyncEval(IDebugEventCallback2 pExprCallback)
        {
            IEngineCallback engineCallback;
            if (pExprCallback != null)
            {
                engineCallback = new EngineCallback(_engine, pExprCallback);
            }
            else
            {
                engineCallback = _engine.Callback;
            }

            Task evalTask = Task.Run(async () =>
            {
                await Eval();
            });

            Action<Task> onComplete = (Task t) =>
            {
                engineCallback.OnExpressionEvaluationComplete(this);
            };
            evalTask.ContinueWith(onComplete, TaskContinuationOptions.ExecuteSynchronously);
        }

        public void AsyncError(IDebugEventCallback2 pExprCallback, IDebugProperty2 error)
        {
            AsyncErrorImpl(pExprCallback != null ? new EngineCallback(_engine, pExprCallback) : _engine.Callback, this, error);
        }

        public static void AsyncErrorImpl(IEngineCallback engineCallback, IVariableInformation var, IDebugProperty2 error)
        {
            Task.Run(() =>
            {
                engineCallback.OnExpressionEvaluationComplete(var, error);
            });
        }

        public void SyncEval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            Task eval = Task.Run(async () =>
            {
                await Eval(dwFlags);
            });
            eval.Wait();
        }

        public ThreadContext ThreadContext { get; }
        public IVariableInformation FindChildByName(string name)
        {
            EnsureChildren();
            if (CountChildren == 0)
            {
                return null;
            }
            Debug.Assert(Children != null, "Failed to find children");
            IVariableInformation var = Children.Find(c => c.Name == name);
            if (var != null)
            {
                return var;
            }
            VariableInformation baseChild = null;
            //var = Array.Find(Children, (c) => c.VariableNodeType == NodeType.BaseClass && (baseChild = c.FindChildByName(name)) != null);
            return baseChild;
        }

        public string EvalDependentExpression(string expr)
        {
            //throw new System.NotImplementedException();
            return null;
        }
        public void Assign(string expression)
        {
            _engine.DebuggedProcess.CommandFactory.ExecCommand($"{FullName()}={expression}");
        }

        public async Task Eval(enum_EVALFLAGS dwFlags = (enum_EVALFLAGS) 0)
        {
            var variable = _engine.DebuggedProcess.ThreadCache.GetVariable(Client.Id, Name);

            var val = String.Empty;
            if (variable != null && !variable.Value.Contains("roAssociativeArray"))
                val = variable.Value;
            else
                val = await _engine.DebuggedProcess.CommandFactory.Print(FullName());

            foreach (var node in mapper)
            {
                if (val.StartsWith(node.Key))
                {
                    VariableNodeType = node.Value;

                    val = val.Replace(node.Key, "").Trim();
                }
            }

            Value = val;

            switch (VariableNodeType)
            {
                case NodeType.Field:
                    var parts = val.Split(new char[] {'\n', '\r'});
                    foreach (var part in parts)
                    {
                        if (part.Contains(":"))
                        {
                            var vals = part.Split(':');
                            Children.Add(new VariableInformation(vals[0].Trim(), this));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}