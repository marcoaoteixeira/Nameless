namespace Nameless.Results;

/// <summary>
///     <see cref="Result{T}"/> extension methods.
/// </summary>
public static class ResultExtensions {
    /// <typeparam name="T">
    ///     The result type.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="Result{T}"/> instance.
    /// </param>
    extension<T>(Result<T> self) {
        /// <summary>
        ///     Whether it represents a failed result.
        /// </summary>
        public bool Failure => !self.Success;
    }
}
