namespace Nameless.Validation;

/// <summary>
/// Validation exception.
/// </summary>
public class ValidationException : Exception {
    /// <summary>
    /// Gets the validation result.
    /// </summary>
    public ValidationResult Result { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/>
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="result"/> is <c>null</c>.
    /// </exception>
    public ValidationException(ValidationResult result)
        => Result = result ?? throw new ArgumentNullException(nameof(result));

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/>
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="result"/> is <c>null</c>.
    /// </exception>
    public ValidationException(ValidationResult result, string message)
        : base(message)
        => Result = result ?? throw new ArgumentNullException(nameof(result));

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/>
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The inner exception.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="result"/> is <c>null</c>.
    /// </exception>
    public ValidationException(ValidationResult result, string message, Exception inner)
        : base(message, inner)
        => Result = result ?? throw new ArgumentNullException(nameof(result));
}