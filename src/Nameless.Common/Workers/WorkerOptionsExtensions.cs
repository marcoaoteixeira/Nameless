namespace Nameless.Workers;

/// <summary>
///     <see cref="WorkerOptions"/> extensions methods.
/// </summary>
public static class WorkerOptionsExtensions {
    /// <param name="self">
    ///     The current <see cref="WorkerOptions"/> instance.
    /// </param>
    extension(WorkerOptions self) {
        /// <summary>
        ///     Whether the worker is disabled.
        /// </summary>
        public bool IsDisabled => !self.IsEnabled;
    }
}