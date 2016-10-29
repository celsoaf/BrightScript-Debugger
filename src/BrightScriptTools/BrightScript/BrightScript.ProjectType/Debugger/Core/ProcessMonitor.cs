using System;
using System.Threading;

namespace BrightScript.Debugger.Core
{
    public class ProcessMonitor : IDisposable
    {
        private readonly TimeSpan _EXIT_POLL_DELTA = TimeSpan.FromMilliseconds(200);
        private int _processId;
        private Timer _exitMonitorTimer;

        public ProcessMonitor(int processId)
        {
            if (!PlatformUtilities.IsLinux() && !PlatformUtilities.IsOSX())
            {
                throw new NotImplementedException();
            }

            _processId = processId;
        }

        public void Start()
        {
            _exitMonitorTimer = new Timer(MonitorForExit, null, TimeSpan.FromMilliseconds(0), _EXIT_POLL_DELTA);
        }

        public event EventHandler ProcessExited;

        private bool HasExited()
        {
            return !UnixUtilities.IsProcessRunning(_processId);
        }

        private void MonitorForExit(object o)
        {
            if (HasExited())
            {
                _exitMonitorTimer.Dispose();
                ProcessExited?.Invoke(this, null);
            }
        }

        public void Dispose()
        {
            _exitMonitorTimer?.Dispose();
        }
    }
}