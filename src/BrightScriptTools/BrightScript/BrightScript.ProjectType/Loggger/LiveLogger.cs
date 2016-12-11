using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
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
        private static DateTime s_initTime;

        private LiveLogger()
        {
            s_initTime = DateTime.Now;
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
            string fullLine = String.Format(CultureInfo.CurrentCulture, "({0}) {1}", (int)(DateTime.Now - s_initTime).TotalMilliseconds, message);
            Debug.WriteLine(fullLine);

            var pane = OutputWindowRedirector.Get(ServiceProvider.GlobalProvider, LiveDiagnosticLogPaneGuid, LiveDiagnosticLogPaneName);
            if (pane != null)
            {
                pane.WriteLine(fullLine);
            }
        }

        /// <summary>
        /// If logging is enabled, writes a block of text which may contain newlines to the log
        /// </summary>
        /// <param name="prefix">[Optional] Prefix to put on the front of each line</param>
        /// <param name="textBlock">Block of text to write</param>
        public static void WriteTextBlock(string prefix, string textBlock)
        {
            Instance.WriteTextBlockImpl(prefix, textBlock);
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // Disable inlining since logging is off by default, and we want to allow the public method to be inlined
        private void WriteLineImpl(string format, object[] args)
        {
            LogMessage(string.Format(CultureInfo.CurrentCulture, format, args));
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // Disable inlining since logging is off by default, and we want to allow the public method to be inlined
        private void WriteTextBlockImpl(string prefix, string textBlock)
        {
            using (var reader = new StringReader(textBlock))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    if (!string.IsNullOrEmpty(prefix))
                        LogMessage(prefix + line);
                    else
                        LogMessage(line);
                }
            }
        }
    }
}