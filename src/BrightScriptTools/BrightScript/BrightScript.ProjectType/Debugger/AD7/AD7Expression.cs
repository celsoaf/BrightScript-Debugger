﻿using System;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Interfaces;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    // This class represents a succesfully parsed expression to the debugger. 
    // It is returned as a result of a successful call to IDebugExpressionContext2.ParseText
    // It allows the debugger to obtain the values of an expression in the debuggee. 
    // For the purposes of this sample, this means obtaining the values of locals and parameters from a stack frame.
    public class AD7Expression : IDebugExpression2
    {
        private AD7Engine _engine;
        private IVariableInformation _var;

        internal AD7Expression(AD7Engine engine, IVariableInformation var)
        {
            _engine = engine;
            _var = var;
        }

        #region IDebugExpression2 Members

        // This method cancels asynchronous expression evaluation as started by a call to the IDebugExpression2::EvaluateAsync method.
        int IDebugExpression2.Abort()
        {
            throw new NotImplementedException();
        }

        // This method evaluates the expression asynchronously.
        // This method should return immediately after it has started the expression evaluation. 
        // When the expression is successfully evaluated, an IDebugExpressionEvaluationCompleteEvent2 
        // must be sent to the IDebugEventCallback2 event callback
        //
        // This is primarily used for the immediate window
        int IDebugExpression2.EvaluateAsync(enum_EVALFLAGS dwFlags, IDebugEventCallback2 pExprCallback)
        {
            if (((dwFlags & enum_EVALFLAGS.EVAL_NOSIDEEFFECTS) != 0 && (dwFlags & enum_EVALFLAGS.EVAL_ALLOWBPS) == 0) && _var.IsVisualized)
            {
                IVariableInformation variable = null; // _engine.DebuggedProcess.Natvis.Cache.Lookup(_var);
                if (variable == null)
                {
                    _var.AsyncError(pExprCallback, new AD7ErrorProperty(_var.Name, ResourceStrings.NoSideEffectsVisualizerMessage));
                }
                else
                {
                    _var = variable;    // use the old value
                    Task.Run(() =>
                    {
                        new EngineCallback(_engine, pExprCallback).OnExpressionEvaluationComplete(variable);
                    });
                }
            }
            else
            {
                _var.AsyncEval(pExprCallback);
            }
            return VSConstants.S_OK;
        }

        // This method evaluates the expression synchronously.
        int IDebugExpression2.EvaluateSync(enum_EVALFLAGS dwFlags, uint dwTimeout, IDebugEventCallback2 pExprCallback, out IDebugProperty2 ppResult)
        {
            ppResult = null;
            if ((dwFlags & enum_EVALFLAGS.EVAL_NOSIDEEFFECTS) != 0 && _var.IsVisualized)
            {
                IVariableInformation variable = _var;
                if (variable == null)
                {
                    ppResult = new AD7ErrorProperty(_var.Name, ResourceStrings.NoSideEffectsVisualizerMessage);
                }
                else
                {
                    _var = variable;
                    ppResult = new AD7Property(_engine, _var);
                }
                return VSConstants.S_OK;
            }

            _var.SyncEval(dwFlags);
            ppResult = new AD7Property(_engine, _var);
            return VSConstants.S_OK;
        }

        #endregion
    }
}