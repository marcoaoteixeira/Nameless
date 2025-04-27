namespace Nameless.Validation;

/// <summary>
/// Defines methods to implement a validation service.
/// </summary>
public interface IValidationService {
    /// <summary>
    /// Validates a value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="throwOnFailure">Whether it should throw an exception on validation failure.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the validation routing execution.</returns>
    Task<ValidationResult> ValidateAsync<TValue>(TValue value, bool throwOnFailure, CancellationToken cancellationToken)
        where TValue : class;
}