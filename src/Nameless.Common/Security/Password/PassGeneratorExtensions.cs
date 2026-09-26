namespace Nameless.Security.Password;

/// <summary>
///     <see cref="IPassGenerator"/> extension methods
/// </summary>
public static class PassGeneratorExtensions {
    /// <param name="self">
    ///     The current <see cref="IPassGenerator"/> instance.
    /// </param>
    extension(IPassGenerator self) {
        /// <summary>
        ///     Generates a password.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the asynchronous
        ///     execution, where the result is the generated password.
        /// </returns>
        public Task<string> GenerateAsync(CancellationToken cancellationToken) {
            return self.GenerateAsync(arguments: new Arguments(), cancellationToken);
        }
    }
}
