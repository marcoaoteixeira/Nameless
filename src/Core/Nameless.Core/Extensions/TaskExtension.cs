namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="Task"/>.
    /// </summary>
    public static class TaskExtension {

        #region Public Static Methods

        /// <summary>
        /// Checks if the <see cref="Task"/> didn't thrown an exception, was canceled, was not faulted and is completed.
        /// </summary>
        /// <param name="self">The <see cref="Task"/> source</param>
        /// <returns><c>true</c> if can continue; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool CanContinue(this Task self) {
            Prevent.Null(self, nameof(self));

            return self.Exception == default && !self.IsCanceled && !self.IsFaulted && self.IsCompleted;
        }

        #endregion
    }
}
