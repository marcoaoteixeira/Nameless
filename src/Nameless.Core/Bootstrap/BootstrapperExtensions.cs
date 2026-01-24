namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IBootstrapper"/> extension methods.
/// </summary>
public static class BootstrapperExtensions {
    /// <param name="self">
    ///     The current <see cref="IBootstrapper"/> instance.
    /// </param>
    extension(IBootstrapper self) {
        /// <summary>
        ///     Asynchronously executes the bootstrap operation.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous execution operation.
        /// </returns>
        public Task ExecuteAsync(CancellationToken cancellationToken) {
            return self.ExecuteAsync(context: [], cancellationToken);
        }
    }
}