namespace Nameless.Validation;

/// <summary>
///     <see cref="IValidationService"/> extension methods.
/// </summary>
public static class ValidationExtensions {
    private static readonly Dictionary<string, object> EmptyContext = [];

    /// <param name="self">
    ///     The current <see cref="IValidationService"/> instance.
    /// </param>
    extension(IValidationService self) {
        /// <summary>
        ///     Validates the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> that represents the asynchronous
        ///     execution, where the result is the validation result of the
        ///     value.
        /// </returns>
        public Task<ValidationResult> ValidateAsync(object value, CancellationToken cancellationToken) {
            return self.ValidateAsync(value, EmptyContext, cancellationToken);
        }
    }
}
