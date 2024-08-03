namespace Nameless {
    /// <summary>
    /// <see cref="Task"/> extension methods.
    /// </summary>
    public static class TaskExtension {
        #region Public Static Methods

        /// <summary>
        /// Checks if the <see cref="Task"/> didn't thrown an exception, was canceled, was not faulted and is completed.
        /// </summary>
        /// <param name="self">The <see cref="Task"/> source</param>
        /// <returns><c>true</c> if it can continue; otherwise <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool CanContinue(this Task self)
            => self.Exception is null && self is { IsCanceled: false, IsFaulted: false, IsCompleted: true };

        #endregion
    }
}
