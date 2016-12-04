using System;
using System.Diagnostics;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using Microsoft.MIDebugEngine;
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
            AD7ProgramDestroyEvent eventObject = new AD7ProgramDestroyEvent(exitCode);

            try
            {
                Send(eventObject, AD7ProgramDestroyEvent.IID, null);
            }
            catch (InvalidOperationException)
            {
                // If debugging has already stopped, this can throw
            }
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
            IDebugProgram2 program = _engine;
            if (!_engine.ProgramCreateEventSent)
            {
                // Any events before programe create shouldn't include the program
                program = null;
            }

            Send(eventObject, iidEvent, program, thread);
        }

        public void OnExpressionEvaluationComplete(IVariableInformation var, IDebugProperty2 prop = null)
        {
            throw new System.NotImplementedException();
        }

        public void OnThreadExit(DebuggedThread debuggedThread, uint exitCode)
        {
            Debug.Assert(_engine.DebuggedProcess.WorkerThread.IsPollThread());

            AD7Thread ad7Thread = (AD7Thread)debuggedThread.Client;
            Debug.Assert(ad7Thread != null);

            AD7ThreadDestroyEvent eventObject = new AD7ThreadDestroyEvent(exitCode);

            Send(eventObject, AD7ThreadDestroyEvent.IID, ad7Thread);
        }

        public void OnThreadStart(DebuggedThread debuggedThread)
        {
            Debug.Assert(_engine.DebuggedProcess.WorkerThread.IsPollThread());

            // This will get called when the entrypoint breakpoint is fired because the engine sends a thread start event
            // for the main thread of the application.

            AD7ThreadCreateEvent eventObject = new AD7ThreadCreateEvent();
            Send(eventObject, AD7ThreadCreateEvent.IID, (IDebugThread2)debuggedThread.Client);
        }
    }
}