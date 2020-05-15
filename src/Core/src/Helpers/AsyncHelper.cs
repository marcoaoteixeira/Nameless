using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Helpers {

    /// <summary>
    /// Asynchronous helper.
    /// </summary>
    public static class AsyncHelper {
        #region Private Static Read-Only Fields

        private static readonly TaskFactory CurrentTaskFactory = new TaskFactory (CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <param name="function">The async method.</param>
        public static void RunSync (Func<Task> function) {
            var cultureUI = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            CurrentTaskFactory.StartNew (() => {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUI;
                return function ();
            })
            .Unwrap ()
            .GetAwaiter ()
            .GetResult ();
        }

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The async method.</param>
        public static TResult RunSync<TResult> (Func<Task<TResult>> function) {
            var cultureUI = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            return CurrentTaskFactory.StartNew (() => {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUI;
                return function ();
            })
            .Unwrap ()
            .GetAwaiter ()
            .GetResult ();
        }

        #endregion
    }
}