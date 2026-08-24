namespace Nameless.Workers;

/// <summary>
///     <see cref="PeriodicWorkerOptions"/> extensions methods.
/// </summary>
public static class PeriodicWorkerOptionsExtensions {
    /// <param name="self">
    ///     The current <see cref="PeriodicWorkerOptions"/> instance.
    /// </param>
    extension(PeriodicWorkerOptions self) {
        /// <summary>
        ///     Whether the worker is disabled.
        /// </summary>
        public bool IsDisabled => !self.IsEnabled;
    }
}