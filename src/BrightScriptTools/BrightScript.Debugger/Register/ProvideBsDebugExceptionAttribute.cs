using System.Linq;
using BrightScript.Debugger.AD7;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Register
{
    public sealed class ProvideBsDebugExceptionAttribute : ProvideDebugExceptionAttribute
    {
        public readonly string ExceptionName;

        public ProvideBsDebugExceptionAttribute(params string[] exceptionPath) : base(AD7Engine.DebugEngineId, "BrightScript Exceptions", exceptionPath) {
            State = enum_EXCEPTION_STATE.EXCEPTION_NONE;
            ExceptionName = exceptionPath.LastOrDefault();
        }
    }
}