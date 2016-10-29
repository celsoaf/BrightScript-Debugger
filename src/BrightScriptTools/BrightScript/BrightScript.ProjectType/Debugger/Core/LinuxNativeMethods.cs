using System.Runtime.InteropServices;

namespace BrightScript.Debugger.Core
{
    internal class LinuxNativeMethods
    {
        private const string Libc = "libc";

        [DllImport(Libc, EntryPoint = "kill", SetLastError = true)]
        internal static extern int Kill(int pid, int mode);

        [DllImport(Libc, EntryPoint = "mkfifo", SetLastError = true)]
        internal static extern int MkFifo(byte[] name, int mode);

        [DllImport(Libc, EntryPoint = "geteuid", SetLastError = true)]
        internal static extern uint GetEUid();
    }
}