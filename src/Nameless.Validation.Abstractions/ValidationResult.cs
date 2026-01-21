namespace Nameless.Validation;

/// <summary>
///     Represents the validation execution result.
/// </summary>
public record ValidationResult {
    /// <summary>
    ///     Gets the errors from the validation execution.
    /// </summary>
    public ValidationError[] Errors { get; } = [];

    /// <summary>
    ///     Whether the validation was successful or not.
    /// </summary>
    public bool Success => Errors.Length == 0;

    private ValidationResult(ValidationError[] errors) {
        Errors = errors;
    }

    /// <summary>
    ///     Creates a new <see cref="ValidationResult"/>.
    /// </summary>
    /// <returns>
    ///     The validation result instance.
    /// </returns>
    public static ValidationResult Create(params IEnumerable<ValidationError> errors) {
        return new ValidationResult([.. errors]);
    }
}