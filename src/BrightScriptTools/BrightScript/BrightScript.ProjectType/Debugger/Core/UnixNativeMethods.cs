using System.Runtime.InteropServices;

namespace BrightScript.Debugger.Core
{
    internal class UnixNativeMethods
    {
        private const string Libc = "libc";

        [DllImport(Libc, EntryPoint = "mkfifo", SetLastError = true)]
        internal static extern int MkFifo(byte[] name, int mode);

        [DllImport(Libc, EntryPoint = "geteuid", SetLastError = true)]
        internal static extern uint GetEUid();

        [DllImport(Libc, EntryPoint = "getpgid", SetLastError = true)]
        internal static extern int GetPGid(int pid);
    }
}