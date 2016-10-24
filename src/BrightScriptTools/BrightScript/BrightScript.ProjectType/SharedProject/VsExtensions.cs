using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;
using BrightScript.SharedProject;
using VsShellUtil = Microsoft.VisualStudio.Shell.VsShellUtilities;


namespace BrightScript
{
    public static class VsExtensions
    {
        internal static UIThreadBase GetUIThread(this IServiceProvider serviceProvider)
        {
            var uiThread = (UIThreadBase)serviceProvider.GetService(typeof(UIThreadBase));
            if (uiThread == null)
            {
                Trace.TraceWarning("Returning NoOpUIThread instance from GetUIThread");
                Debug.Assert(VsShellUtil.ShellIsShuttingDown, "No UIThread service but shell is not shutting down");
                return new NoOpUIThread();
            }
            return uiThread;
        }

        #region NoOpUIThread class

        /// <summary>
        /// Provides a no-op implementation of <see cref="UIThreadBase"/> that will
        /// not execute any tasks.
        /// </summary>
        private sealed class NoOpUIThread : MockUIThreadBase
        {
            public override void Invoke(Action action) { }

            public override T Invoke<T>(Func<T> func)
            {
                return default(T);
            }

            public override Task InvokeAsync(Action action)
            {
                return Task.FromResult<object>(null);
            }

            public override Task<T> InvokeAsync<T>(Func<T> func)
            {
                return Task.FromResult<T>(default(T));
            }

            public override Task InvokeTask(Func<Task> func)
            {
                return Task.FromResult<object>(null);
            }

            public override Task<T> InvokeTask<T>(Func<Task<T>> func)
            {
                return Task.FromResult<T>(default(T));
            }

            public override void MustBeCalledFromUIThreadOrThrow() { }

            public override bool InvokeRequired
            {
                get { return false; }
            }
        }

        #endregion
    }
}