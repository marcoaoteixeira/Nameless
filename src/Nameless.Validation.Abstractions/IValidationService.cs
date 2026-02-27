namespace Nameless.Validation;

/// <summary>
///     Defines methods to implement a validation service.
/// </summary>
public interface IValidationService {
    /// <summary>
    ///     Validates a value.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <param name="context">
    ///     The validation context.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{T}"/> representing the asynchronous execution.
    ///     The result of the task will be the validation result.
    /// </returns>
    Task<ValidationResult> ValidateAsync(object value, IDictionary<string, object> context, CancellationToken cancellationToken);
}