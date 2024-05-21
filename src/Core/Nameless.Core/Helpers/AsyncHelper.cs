namespace Nameless.Helpers {
    public static class AsyncHelper {
        #region Public Static Methods

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <param name="function">The async method.</param>
        /// <param name="millisecondsTimeout">Timeout in milliseconds. Value <c>-1</c> indicates <c>no timeout</c>.</param>
        public static void RunSync(Func<Task> function, int millisecondsTimeout = -1)
            => Task.WaitAny(
                tasks: [function()],
                cancellationToken: GenerateCancellationToken(millisecondsTimeout)
            );

        /// <summary>
        /// Executes an asynchronous method synchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The async method.</param>
        /// <param name="millisecondsTimeout">Timeout in milliseconds. Value <c>-1</c> indicates <c>no timeout</c>.</param>
        public static TResult RunSync<TResult>(Func<Task<TResult>> function, int millisecondsTimeout = -1) {
            var task = function();

            Task.WaitAny(
                tasks: [task],
                cancellationToken: GenerateCancellationToken(millisecondsTimeout)
            );

            return task.Result;
        }

        #endregion

        #region Private Static Methods

        private static CancellationToken GenerateCancellationToken(int millisecondsTimeout) {
            if (millisecondsTimeout <= 0) {
                return default;
            }

            var cts = new CancellationTokenSource(millisecondsTimeout);
            cts.Token.Register(DisposeCancellationTokenSource, cts);

            return cts.Token;
        }

        private static void DisposeCancellationTokenSource(object? state) {
            if (state is CancellationTokenSource cts) {
                cts.Dispose();
            }
        }

        #endregion
    }
}