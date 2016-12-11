namespace BrightScript.Debugger.Transport
{
    public interface ITransportCallback
    {
        void OnStdOutLine(string line);
        void OnDebuggerProcessExit(string exitCode);
    }
}