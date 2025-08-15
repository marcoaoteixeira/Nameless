namespace Nameless.Validation;

/// <summary>
///     Validation exception.
/// </summary>
public class ValidationException : Exception {
    /// <summary>
    ///     Gets the validation result.
    /// </summary>
    public ValidationResult Result { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationException" />
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="result" /> is <see langword="null"/>.
    /// </exception>
    public ValidationException(ValidationResult result) {
        Result = Guard.Against.Null(result);
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationException" />
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="result" /> is <see langword="null"/>.
    /// </exception>
    public ValidationException(ValidationResult result, string message)
        : base(message) {
        Result = Guard.Against.Null(result);
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationException" />
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The inner exception.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="result" /> is <see langword="null"/>.
    /// </exception>
    public ValidationException(ValidationResult result, string message, Exception inner)
        : base(message, inner) {
        Result = Guard.Against.Null(result);
    }
}