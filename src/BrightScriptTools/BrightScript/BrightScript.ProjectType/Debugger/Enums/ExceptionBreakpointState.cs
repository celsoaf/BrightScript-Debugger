using System;

namespace BrightScript.Debugger.Enums
{
    [Flags]
    public enum ExceptionBreakpointState
    {
        None = 0,
        BreakUserHandled = 0x1,
        BreakThrown = 0x2
    }
}