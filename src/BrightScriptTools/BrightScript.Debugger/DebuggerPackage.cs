using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Register;
using Microsoft.VisualStudio.Shell;

namespace BrightScript.Debugger
{
    [ProvideDebugEngine("BrightScript Debugging", typeof(AD7ProgramProvider), typeof(AD7Engine), AD7Engine.DebugEngineId, setNextStatement: false, hitCountBp: true, justMyCodeStepping: false)]
    // Keep declared exceptions in sync with those given default values in NodeDebugger.GetDefaultExceptionTreatments()
    [ProvideBsDebugException()]
    [ProvideBsDebugException("Error")]
    // VS2015's exception manager uses a different nesting structure, so it's necessary to register Error explicitly.
    [ProvideBsDebugException("Error", "Error")]
    public class DebuggerPackage : Package
    {
        
    }
}