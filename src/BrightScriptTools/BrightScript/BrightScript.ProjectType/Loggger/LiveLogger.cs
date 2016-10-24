using System;
using System.Diagnostics;
using BrightScript.SharedProject;
using Microsoft.VisualStudio.Shell;

namespace BrightScript.Loggger
{
    /// <summary>
    /// An efficient logger that logs diagnostic messages using Debug.WriteLine.
    /// Additionally logs messages to the NTVS Diagnostics task pane if option is enabled.
    /// </summary>
    internal sealed class LiveLogger
    {
        private static Guid LiveDiagnosticLogPaneGuid = new Guid("{66386208-2E7E-4B93-A852-D1A32EE00107}");
        private const string LiveDiagnosticLogPaneName = "BrightScript Tools Live Diagnostics";

        private static volatile LiveLogger _instance;
        private static object _loggerLock = new object();

        private LiveLogger()
        {
        }

        private static LiveLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_loggerLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LiveLogger();
                        }
                    }
                }
                return _instance;
            }
        }

        public static void WriteLine(string message, Type category)
        {
            WriteLine("{0}: {1}", category.Name, message);
        }

        public static void WriteLine(string message)
        {
            var str = String.Format("[{0}] {1}", DateTime.UtcNow.TimeOfDay, message);
            Instance.LogMessage(str);
        }

        public static void WriteLine(string format, params object[] args)
        {
            var str = String.Format(format, args);
            WriteLine(str);
        }

        private void LogMessage(string message)
        {
            Debug.WriteLine(message);

            var pane = OutputWindowRedirector.Get(ServiceProvider.GlobalProvider, LiveDiagnosticLogPaneGuid, LiveDiagnosticLogPaneName);
            if (pane != null)
            {
                pane.WriteLine(message);
            }
        }
    }
}