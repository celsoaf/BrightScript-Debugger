using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BrightScript.Debugger.Exceptions
{
    public class MIDebuggerInitializeFailedException : Exception
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly IReadOnlyList<string> OutputLines;
        private readonly string _debuggerName;
        private readonly IReadOnlyList<string> _errorLines;
        private string _message;

        public MIDebuggerInitializeFailedException(string debuggerName, IReadOnlyList<string> errorLines, IReadOnlyList<string> outputLines)
        {
            this.OutputLines = outputLines;
            _debuggerName = debuggerName;
            _errorLines = errorLines;
        }

        public override string Message
        {
            get
            {
                if (_message == null)
                {
                    if (_errorLines.Any(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        _message = string.Format(CultureInfo.InvariantCulture, MICoreResources.Error_DebuggerInitializeFailed_StdErr, _debuggerName, string.Join("\r\n", _errorLines));
                    }
                    else
                    {
                        _message = string.Format(CultureInfo.InvariantCulture, MICoreResources.Error_DebuggerInitializeFailed_NoStdErr, _debuggerName);
                    }
                }

                return _message;
            }
        }
    }
}