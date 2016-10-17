﻿using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    // This class represents the information that describes a bound breakpoint.
    internal class AD7BreakpointResolution : IDebugBreakpointResolution2
    {
        private AD7Engine m_engine;
        private AD7DocumentContext m_documentContext;

        public AD7BreakpointResolution(AD7Engine engine, AD7DocumentContext documentContext)
        {
            m_engine = engine;
            m_documentContext = documentContext;
        }

        #region IDebugBreakpointResolution2 Members

        // Gets the type of the breakpoint represented by this resolution. 
        int IDebugBreakpointResolution2.GetBreakpointType(enum_BP_TYPE[] pBPType)
        {
            // The sample engine only supports code breakpoints.
            pBPType[0] = enum_BP_TYPE.BPT_CODE;
            return VSConstants.S_OK;
        }

        // Gets the breakpoint resolution information that describes this breakpoint.
        int IDebugBreakpointResolution2.GetResolutionInfo(enum_BPRESI_FIELDS dwFields, BP_RESOLUTION_INFO[] pBPResolutionInfo)
        {
            if ((dwFields & enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION) != 0)
            {
                // The sample engine only supports code breakpoints.
                BP_RESOLUTION_LOCATION location = new BP_RESOLUTION_LOCATION();
                location.bpType = (uint)enum_BP_TYPE.BPT_CODE;

                // The debugger will not QI the IDebugCodeContex2 interface returned here. We must pass the pointer
                // to IDebugCodeContex2 and not IUnknown.
                AD7MemoryAddress codeContext = new AD7MemoryAddress(m_engine);
                codeContext.SetDocumentContext(m_documentContext);
                location.unionmember1 = Marshal.GetComInterfaceForObject(codeContext, typeof(IDebugCodeContext2));
                pBPResolutionInfo[0].bpResLocation = location;
                pBPResolutionInfo[0].dwFields |= enum_BPRESI_FIELDS.BPRESI_BPRESLOCATION;
            }

            if ((dwFields & enum_BPRESI_FIELDS.BPRESI_PROGRAM) != 0)
            {
                pBPResolutionInfo[0].pProgram = (IDebugProgram2)m_engine;
                pBPResolutionInfo[0].dwFields |= enum_BPRESI_FIELDS.BPRESI_PROGRAM;
            }

            return VSConstants.S_OK;
        }

        #endregion
    }

    class AD7ErrorBreakpointResolution : IDebugErrorBreakpointResolution2
    {
        private string m_Message;
        private enum_BP_ERROR_TYPE m_errorType;

        public AD7ErrorBreakpointResolution(string msg, enum_BP_ERROR_TYPE errorType = enum_BP_ERROR_TYPE.BPET_GENERAL_WARNING)
        {
            m_Message = msg;
            m_errorType = errorType;
        }

        #region IDebugErrorBreakpointResolution2 Members

        int IDebugErrorBreakpointResolution2.GetBreakpointType(enum_BP_TYPE[] pBPType)
        {
            pBPType[0] = enum_BP_TYPE.BPT_CODE;

            return VSConstants.S_OK;
        }

        int IDebugErrorBreakpointResolution2.GetResolutionInfo(enum_BPERESI_FIELDS dwFields, BP_ERROR_RESOLUTION_INFO[] info)
        {
            if ((dwFields & enum_BPERESI_FIELDS.BPERESI_BPRESLOCATION) != 0)
            {
            }
            if ((dwFields & enum_BPERESI_FIELDS.BPERESI_PROGRAM) != 0)
            {
            }
            if ((dwFields & enum_BPERESI_FIELDS.BPERESI_THREAD) != 0)
            {
            }
            if ((dwFields & enum_BPERESI_FIELDS.BPERESI_MESSAGE) != 0)
            {
                info[0].dwFields |= enum_BPERESI_FIELDS.BPERESI_MESSAGE;
                info[0].bstrMessage = m_Message;
            }
            if ((dwFields & enum_BPERESI_FIELDS.BPERESI_TYPE) != 0)
            {
                info[0].dwFields |= enum_BPERESI_FIELDS.BPERESI_TYPE;
                info[0].dwType = m_errorType;
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}