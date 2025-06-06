namespace Nameless.Results;

/// <summary>
/// Represents an error.
/// </summary>
public readonly record struct Error {
    /// <summary>
    /// Gets the description of the error.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    ///     Do not use type constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     if parameterless constructor is called.
    /// </exception>
    public Error() {
        throw new InvalidOperationException("Do not use type constructor.");
    }

    private Error(string description, ErrorType type) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);
        Type = type;
    }

    /// <summary>
    /// Creates a validation error with the specified description.
    /// </summary>
    /// <param name="description">The description of the validation error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the validation error.</returns>
    public static Error Validation(string description) {
        return new Error(description, ErrorType.Validation);
    }

    /// <summary>
    /// Creates a missing error with the specified description.
    /// </summary>
    /// <param name="description">The description of the missing error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the validation error.</returns>
    public static Error Missing(string description) {
        return new Error(description, ErrorType.Missing);
    }

    /// <summary>
    /// Creates a conflict error with the specified description.
    /// </summary>
    /// <param name="description">The description of the conflict error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the conflict error.</returns>
    public static Error Conflict(string description) {
        return new Error(description, ErrorType.Conflict);
    }

    /// <summary>
    /// Creates a failure error with the specified description.
    /// </summary>
    /// <param name="description">The description of the failure error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the failure error.</returns>
    public static Error Failure(string description) {
        return new Error(description, ErrorType.Failure);
    }

    /// <summary>
    /// Creates a forbidden error with the specified description.
    /// </summary>
    /// <param name="description">The description of the forbidden error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the forbidden error.</returns>
    public static Error Forbidden(string description) {
        return new Error(description, ErrorType.Forbidden);
    }

    /// <summary>
    /// Creates an unauthorized error with the specified description.
    /// </summary>
    /// <param name="description">The description of the unauthorized error. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> object representing the unauthorized error.</returns>
    public static Error Unauthorized(string description) {
        return new Error(description, ErrorType.Unauthorized);
    }
}