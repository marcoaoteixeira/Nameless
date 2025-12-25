namespace Nameless.Results;

/// <summary>
///     Represents an error.
/// </summary>
public readonly record struct Error {
    /// <summary>
    ///     Gets the error summary.
    /// </summary>
    public string? Summary { get; }

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

    private Error(string? summary, string description, ErrorType type) {
        Summary = summary;
        Description = description;
        Type = type;
    }

    /// <summary>
    ///     Creates a validation error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the validation error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Validation(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Validation);
    }

    /// <summary>
    /// Creates a missing error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the validation error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Missing(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Missing);
    }

    /// <summary>
    /// Creates a conflict error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the conflict error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Conflict(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Conflict);
    }

    /// <summary>
    /// Creates a failure error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the failure error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Failure(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Failure);
    }

    /// <summary>
    /// Creates a forbidden error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the forbidden error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Forbidden(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Forbidden);
    }

    /// <summary>
    /// Creates an unauthorized error with the specified description.
    /// </summary>
    /// <param name="description">
    ///     The error description.
    /// </param>
    /// <param name="summary">
    ///     The error summary.
    /// </param>
    /// <returns>
    ///     An <see cref="Error"/> object representing the unauthorized error.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="description"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="description"/> is empty or only whitespaces.
    /// </exception>
    public static Error Unauthorized(string description, string? summary = null) {
        return new Error(summary, description, ErrorType.Unauthorized);
    }
}