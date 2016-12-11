using System;

namespace BrightScript.SharedProject
{
    /// <summary>
    /// Base class that can receive output from <see cref="ProcessOutput"/>.
    /// 
    /// If this class implements <see cref="IDisposable"/>, it will be disposed
    /// when the <see cref="ProcessOutput"/> object is disposed.
    /// </summary>
    abstract class Redirector
    {
        /// <summary>
        /// Called when a line is written to standard output.
        /// </summary>
        /// <param name="line">The line of text, not including the newline. This
        /// is never null.</param>
        public abstract void WriteLine(string line);

        /// <summary>
        /// Called when a line is written to standard error.
        /// </summary>
        /// <param name="line">The line of text, not including the newline. This
        /// is never null.</param>
        public abstract void WriteErrorLine(string line);

        /// <summary>
        /// Called when output is written that should be brought to the user's
        /// attention. The default implementation does nothing.
        /// </summary>
        public virtual void Show()
        {
        }

        /// <summary>
        /// Called when output is written that should be brought to the user's
        /// immediate attention. The default implementation does nothing.
        /// </summary>
        public virtual void ShowAndActivate()
        {
        }
    }
}