using System.Globalization;

namespace BrightScript.Debugger.Exceptions
{
    public class UnexpectedMIResultException : MIException
    {
        // We want to have a message which is vaguely reasonable if it winds up getting converted to an HRESULT. So
        // we will use take this one.
        //    MessageId: COMQC_E_BAD_MESSAGE
        //
        //    MessageText:
        //      The message is improperly formatted or was damaged in transit
        private const int COMQC_E_BAD_MESSAGE = unchecked((int)0x80110604);
        private readonly string _debuggerName;
        private readonly string _command;
        private readonly string _miError;

        /// <summary>
        /// Creates a new UnexpectedMIResultException
        /// </summary>
        /// <param name="debuggerName">[Required] Name of the underlying MI debugger (ex: 'GDB')</param>
        /// <param name="command">[Required] MI command that was issued</param>
        /// <param name="mi">[Optional] Error message from MI</param>
        public UnexpectedMIResultException(string debuggerName, string command, string mi) : base(COMQC_E_BAD_MESSAGE)
        {
            _debuggerName = debuggerName;
            _command = command;
            _miError = mi;
        }

        public override string Message
        {
            get
            {
                string message = string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_UnexpectedMIOutput, _debuggerName, _command);
                if (!string.IsNullOrWhiteSpace(_miError))
                {
                    message = string.Concat(message, " ", _miError);
                }

                return message;
            }
        }

        public string MIError
        {
            get { return _miError; }
        }
    }
}