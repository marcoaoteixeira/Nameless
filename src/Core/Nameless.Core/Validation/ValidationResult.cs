namespace Nameless.Validation;

/// <summary>
/// Represents the validation execution result.
/// </summary>
public sealed record ValidationResult {
    /// <summary>
    /// Retrieves a empty validation result. (Success)
    /// </summary>
    public static ValidationResult Empty => new();

    /// <summary>
    /// Gets the errors from the validation execution.
    /// </summary>
    public ValidationError[] Errors { get; } = [];

    /// <summary>
    /// Whether the validation was successful or not.
    /// </summary>
    public bool Succeeded => Errors.Length == 0;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="errors">The errors if they exist.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="errors"/> is <c>null</c>.
    /// </exception>
    public ValidationResult(params ValidationError[] errors)
        => Errors = Prevent.Argument.Null(errors);
}