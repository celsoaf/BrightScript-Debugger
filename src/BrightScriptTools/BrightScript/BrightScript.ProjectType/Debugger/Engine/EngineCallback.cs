using System;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class EngineCallback : IEngineCallback
    {
        private readonly IDebugEventCallback2 _eventCallback;
        private readonly AD7Engine _engine;

        public EngineCallback(AD7Engine engine, IDebugEventCallback2 ad7Callback)
        {
            _engine = engine;
            _eventCallback = ad7Callback;
        }

        public void OnErrorImmediate(string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnProcessExit(uint exitCode)
        {
            throw new System.NotImplementedException();
        }

        public void Send(IDebugEvent2 eventObject, string iidEvent, IDebugProgram2 program, IDebugThread2 thread)
        {
            uint attributes;
            Guid riidEvent = new Guid(iidEvent);

            EngineUtils.RequireOk(eventObject.GetAttributes(out attributes));
            EngineUtils.RequireOk(_eventCallback.Event(_engine, null, program, thread, eventObject, ref riidEvent, attributes));
        }

        public void Send(IDebugEvent2 eventObject, string iidEvent, IDebugThread2 thread)
        {
            throw new System.NotImplementedException();
        }

        public void OnExpressionEvaluationComplete(IVariableInformation var, IDebugProperty2 prop = null)
        {
            throw new System.NotImplementedException();
        }
    }
}