using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Models
{
    public class OutputMessage
    {
        public enum Severity
        {
            Error,
            Warning
        };

        public readonly string Message;
        public readonly enum_MESSAGETYPE MessageType;
        public readonly Severity SeverityValue;

        /// <summary>
        /// Error HRESULT to send to the debug package. 0 (S_OK) if there is no associated error code.
        /// </summary>
        public readonly uint ErrorCode;

        public OutputMessage(string message, enum_MESSAGETYPE messageType, Severity severity, uint errorCode = 0)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.SeverityValue = severity;
            this.ErrorCode = errorCode;
        }
    }
}