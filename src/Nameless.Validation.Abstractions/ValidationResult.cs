namespace Nameless.Validation;

/// <summary>
///     Represents the validation execution result.
/// </summary>
public sealed record ValidationResult {
    /// <summary>
    ///     Gets the errors from the validation execution.
    /// </summary>
    public ValidationError[] Errors { get; } = [];

    /// <summary>
    ///     Whether the validation was successful or not.
    /// </summary>
    public bool Succeeded => Errors.Length == 0;

    private ValidationResult(ValidationError[] errors) {
        Errors = errors;
    }

    /// <summary>
    /// Creates a new <see cref="ValidationResult"/> with success status.
    /// </summary>
    /// <returns>The validation result instance.</returns>
    public static ValidationResult Success() {
        return new ValidationResult([]);
    }

    /// <summary>
    /// Creates a new <see cref="ValidationResult"/> with failure status.
    /// </summary>
    /// <param name="errors">The errors.</param>
    /// <returns>The validation result instance.</returns>
    public static ValidationResult Failure(IEnumerable<ValidationError> errors) {
        var array = errors.ToArray();

        return new ValidationResult(Guard.Against.NullOrEmpty(array));
    }
}