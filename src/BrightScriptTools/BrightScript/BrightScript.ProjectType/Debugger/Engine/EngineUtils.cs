using System;
using System.Diagnostics;
using System.Globalization;
using BrightScript.Debugger.Exceptions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    public static class EngineUtils
    {
        public static AD_PROCESS_ID GetProcessId(IDebugProcess2 process)
        {
            AD_PROCESS_ID[] pid = new AD_PROCESS_ID[1];
            EngineUtils.RequireOk(process.GetPhysicalProcessId(pid));
            return pid[0];
        }

        public static void RequireOk(int hr)
        {
            if (hr != 0)
            {
                throw new InvalidOperationException();
            }
        }

        public static AD_PROCESS_ID GetProcessId(IDebugProgram2 program)
        {
            IDebugProcess2 process;
            RequireOk(program.GetProcess(out process));

            return GetProcessId(process);
        }

        public static bool ProcIdEquals(AD_PROCESS_ID pid1, AD_PROCESS_ID pid2)
        {
            if (pid1.ProcessIdType != pid2.ProcessIdType)
            {
                return false;
            }
            else if (pid1.ProcessIdType == (int)enum_AD_PROCESS_ID.AD_PROCESS_ID_SYSTEM)
            {
                return pid1.dwProcessId == pid2.dwProcessId;
            }
            else
            {
                return pid1.guidProcessId == pid2.guidProcessId;
            }
        }

        public static int UnexpectedException(Exception e)
        {
            Debug.Fail("Unexpected exception during Attach");
            return VSConstants.RPC_E_SERVERFAULT;
        }

        public static string GetExceptionDescription(Exception exception)
        {
            if (!ExceptionHelper.IsCorruptingException(exception))
            {
                return exception.Message;
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_CorruptingException, exception.GetType().FullName, exception.StackTrace);
            }
        }

        public static void CheckOk(int hr)
        {
            if (hr != 0)
            {
                throw new MIException(hr);
            }
        }
    }
}