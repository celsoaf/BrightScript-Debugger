﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    // This file contains the various event objects that are sent to the debugger from the sample engine via IDebugEventCallback2::Event.
    // These are used in EngineCallback.cs.
    // The events are how the engine tells the debugger about what is happening in the debuggee process. 
    // There are three base classe the other events derive from: AD7AsynchronousEvent, AD7StoppingEvent, and AD7SynchronousEvent. These 
    // each implement the IDebugEvent2.GetAttributes method for the type of event they represent. 
    // Most events sent the debugger are asynchronous events.

    #region Event base classes

    internal class AD7AsynchronousEvent : IDebugEvent2
    {
        public const uint Attributes = (uint)enum_EVENTATTRIBUTES.EVENT_ASYNCHRONOUS;

        int IDebugEvent2.GetAttributes(out uint eventAttributes)
        {
            eventAttributes = Attributes;
            return VSConstants.S_OK;
        }
    }

    class AD7StoppingEvent : IDebugEvent2
    {
        public const uint Attributes = (uint)enum_EVENTATTRIBUTES.EVENT_ASYNC_STOP;

        int IDebugEvent2.GetAttributes(out uint eventAttributes)
        {
            eventAttributes = Attributes;
            return VSConstants.S_OK;
        }
    }

    class AD7SynchronousEvent : IDebugEvent2
    {
        public const uint Attributes = (uint)enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS;

        int IDebugEvent2.GetAttributes(out uint eventAttributes)
        {
            eventAttributes = Attributes;
            return VSConstants.S_OK;
        }
    }

    class AD7SynchronousStoppingEvent : IDebugEvent2
    {
        public const uint Attributes = (uint)enum_EVENTATTRIBUTES.EVENT_STOPPING | (uint)enum_EVENTATTRIBUTES.EVENT_SYNCHRONOUS;

        int IDebugEvent2.GetAttributes(out uint eventAttributes)
        {
            eventAttributes = Attributes;
            return VSConstants.S_OK;
        }
    }

    #endregion

    // The debug engine (DE) sends this interface to the session debug manager (SDM) when an instance of the DE is created.
    sealed class AD7EngineCreateEvent : AD7AsynchronousEvent, IDebugEngineCreateEvent2
    {
        public const string IID = "FE5B734C-759D-4E59-AB04-F103343BDD06";
        private IDebugEngine2 m_engine;

        AD7EngineCreateEvent(AD7Engine engine)
        {
            m_engine = engine;
        }

        public static void Send(AD7Engine engine)
        {
            AD7EngineCreateEvent eventObject = new AD7EngineCreateEvent(engine);
            engine.Send(eventObject, IID, null);
        }

        int IDebugEngineCreateEvent2.GetEngine(out IDebugEngine2 engine)
        {
            engine = m_engine;

            return VSConstants.S_OK;
        }
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a program is attached to.
    sealed class AD7ProgramCreateEvent : AD7SynchronousEvent, IDebugProgramCreateEvent2
    {
        public const string IID = "96CD11EE-ECD4-4E89-957E-B5D496FC4139";

        internal static void Send(AD7Engine engine)
        {
            AD7ProgramCreateEvent eventObject = new AD7ProgramCreateEvent();
            engine.Send(eventObject, IID, null);
        }
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a program has run to completion
    // or is otherwise destroyed.
    sealed class AD7ProgramDestroyEvent : AD7SynchronousEvent, IDebugProgramDestroyEvent2
    {
        public const string IID = "E147E9E3-6440-4073-A7B7-A65592C714B5";

        readonly uint m_exitCode;
        public AD7ProgramDestroyEvent(uint exitCode)
        {
            m_exitCode = exitCode;
        }

        #region IDebugProgramDestroyEvent2 Members

        int IDebugProgramDestroyEvent2.GetExitCode(out uint exitCode)
        {
            exitCode = m_exitCode;

            return VSConstants.S_OK;
        }

        #endregion
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a thread is created in a program being debugged.
    sealed class AD7ThreadCreateEvent : AD7AsynchronousEvent, IDebugThreadCreateEvent2
    {
        public const string IID = "2090CCFC-70C5-491D-A5E8-BAD2DD9EE3EA";

        internal static void Send(AD7Engine engine)
        {
            AD7ThreadCreateEvent eventObject = new AD7ThreadCreateEvent();
            engine.Send(eventObject, IID);
        }
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a thread has exited.
    sealed class AD7ThreadDestroyEvent : AD7AsynchronousEvent, IDebugThreadDestroyEvent2
    {
        public const string IID = "2C3B7532-A36F-4A6E-9072-49BE649B8541";

        readonly uint m_exitCode;
        public AD7ThreadDestroyEvent(uint exitCode)
        {
            m_exitCode = exitCode;
        }

        #region IDebugThreadDestroyEvent2 Members

        int IDebugThreadDestroyEvent2.GetExitCode(out uint exitCode)
        {
            exitCode = m_exitCode;

            return VSConstants.S_OK;
        }

        #endregion
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) when a program is loaded, but before any code is executed.
    sealed class AD7LoadCompleteEvent : AD7AsynchronousEvent, IDebugLoadCompleteEvent2
    {
        public const string IID = "B1844850-1349-45D4-9F12-495212F5EB0B";

        public AD7LoadCompleteEvent()
        {
        }

        internal static void Send(AD7Engine engine)
        {
            AD7LoadCompleteEvent eventObject = new AD7LoadCompleteEvent();
            engine.Send(eventObject, AD7LoadCompleteEvent.IID);
        }
    }

    // This interface tells the session debug manager (SDM) that an asynchronous break has been successfully completed.
    sealed class AD7AsyncBreakCompleteEvent : AD7StoppingEvent, IDebugBreakEvent2
    {
        public const string IID = "c7405d1d-e24b-44e0-b707-d8a5a4e1641b";
    }

    // This interface is sent by the debug engine (DE) to the session debug manager (SDM) to output a string for debug tracing.
    sealed class AD7OutputDebugStringEvent : AD7AsynchronousEvent, IDebugOutputStringEvent2
    {
        public const string IID = "569c4bb1-7b82-46fc-ae28-4536ddad753e";

        private string m_str;
        public AD7OutputDebugStringEvent(string str)
        {
            m_str = str;
        }

        #region IDebugOutputStringEvent2 Members

        int IDebugOutputStringEvent2.GetString(out string pbstrString)
        {
            pbstrString = m_str;
            return VSConstants.S_OK;
        }

        #endregion
    }

    // This interface is sent when a pending breakpoint has been bound in the debuggee.
    sealed class AD7BreakpointBoundEvent : AD7AsynchronousEvent, IDebugBreakpointBoundEvent2
    {
        public const string IID = "1dddb704-cf99-4b8a-b746-dabb01dd13a0";
        private AD7BoundBreakpoint m_boundBreakpoint;

        public AD7BreakpointBoundEvent(AD7BoundBreakpoint boundBreakpoint)
        {
            m_boundBreakpoint = boundBreakpoint;
        }

        #region IDebugBreakpointBoundEvent2 Members

        int IDebugBreakpointBoundEvent2.EnumBoundBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            IDebugBoundBreakpoint2[] boundBreakpoints = new IDebugBoundBreakpoint2[1];
            boundBreakpoints[0] = m_boundBreakpoint;
            ppEnum = new AD7BoundBreakpointsEnum(boundBreakpoints);
            return VSConstants.S_OK;
        }

        int IDebugBreakpointBoundEvent2.GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBP)
        {
            return ((IDebugBoundBreakpoint2)m_boundBreakpoint).GetPendingBreakpoint(out ppPendingBP);
        }

        #endregion
    }

    // This Event is sent when a breakpoint is hit in the debuggee
    sealed class AD7BreakpointEvent : AD7StoppingEvent, IDebugBreakpointEvent2
    {
        public const string IID = "501C1E21-C557-48B8-BA30-A1EAB0BC4A74";

        IEnumDebugBoundBreakpoints2 m_boundBreakpoints;

        public AD7BreakpointEvent(IDebugBoundBreakpoint2 boundBreakpoint)
        {
            // m_boundBreakpoints = boundBreakpoints;
            this.m_boundBreakpoints = new AD7BoundBreakpointsEnum(new IDebugBoundBreakpoint2[] { boundBreakpoint });
        }

        internal static void Send(AD7Engine engine, IDebugBoundBreakpoint2 boundBreakpoint)
        {
            AD7BreakpointEvent eventObject = new AD7BreakpointEvent(boundBreakpoint);
            engine.Send(eventObject, IID, null);
        }

        #region IDebugBreakpointEvent2 Members

        int IDebugBreakpointEvent2.EnumBreakpoints(out IEnumDebugBoundBreakpoints2 ppEnum)
        {
            ppEnum = m_boundBreakpoints;
            return VSConstants.S_OK;
        }

        #endregion
    }

    // This Event is sent when a step occurs
    sealed class AD7StepCompleteEvent : AD7StoppingEvent, IDebugStepCompleteEvent2
    {
        public const string IID = "0F7F24C1-74D9-4EA6-A3EA-7EDB2D81441D";

        public AD7StepCompleteEvent() { }
    }
}