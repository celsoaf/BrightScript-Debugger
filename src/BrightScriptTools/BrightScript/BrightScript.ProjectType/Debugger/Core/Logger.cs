using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using BrightScript.Loggger;

namespace BrightScript.Debugger.Core
{
    /// <summary>
    /// Class which implements logging. The logging is control by a registry key. If enabled, logging goes to %TMP%\Microsoft.MIDebug.log
    /// </summary>
    public class Logger
    {
        private static bool s_isInitialized;
        private static bool s_isEnabled;
        private static DateTime s_initTime;
        // NOTE: We never clean this up
        private static int s_count;
        private int _id;

        public Logger()
        {
            _id = Interlocked.Increment(ref s_count);

            s_isInitialized = true;
            s_initTime = DateTime.Now;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                s_isEnabled = true;
            }
#endif
        }

        /// <summary>
        /// If logging is enabled, writes a line of text to the log
        /// </summary>
        /// <param name="line">[Required] line to write</param>
        public void WriteLine(string line)
        {
            if (s_isEnabled)
            {
                WriteLineImpl(line);
            }
        }

        /// <summary>
        /// If logging is enabled, writes a line of text to the log
        /// </summary>
        /// <param name="format">[Required] format string</param>
        /// <param name="args">arguments to use in the format string</param>
        public void WriteLine(string format, params object[] args)
        {
            if (s_isEnabled)
            {
                WriteLineImpl(format, args);
            }
        }

        /// <summary>
        /// If logging is enabled, writes a block of text which may contain newlines to the log
        /// </summary>
        /// <param name="prefix">[Optional] Prefix to put on the front of each line</param>
        /// <param name="textBlock">Block of text to write</param>
        public void WriteTextBlock(string prefix, string textBlock)
        {
            if (s_isEnabled)
            {
                WriteTextBlockImpl(prefix, textBlock);
            }
        }

        public static bool IsEnabled
        {
            get { return s_isEnabled; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // Disable inlining since logging is off by default, and we want to allow the public method to be inlined
        private void WriteLineImpl(string line)
        {
            string fullLine = String.Format(CultureInfo.CurrentCulture, "{2}: ({0}) {1}", (int)(DateTime.Now - s_initTime).TotalMilliseconds, line, _id);
            LiveLogger.WriteLine(fullLine);
#if DEBUG
            Debug.WriteLine("MS_MIDebug: " + fullLine);
#endif
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // Disable inlining since logging is off by default, and we want to allow the public method to be inlined
        private void WriteLineImpl(string format, object[] args)
        {
            WriteLineImpl(string.Format(CultureInfo.CurrentCulture, format, args));
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
                        WriteLineImpl(prefix + line);
                    else
                        WriteLineImpl(line);
                }
            }
        }
    }
}