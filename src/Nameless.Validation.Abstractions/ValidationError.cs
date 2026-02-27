namespace Nameless.Validation;

/// <summary>
///     Represents a validation error entry.
/// </summary>
public record ValidationError {
    /// <summary>
    ///     Gets the error message.
    /// </summary>
    public string Error { get; }

    /// <summary>
    ///     Gets the error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Gets the member responsible for the validation error.
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="error">
    ///     The error message.
    /// </param>
    /// <param name="code">
    ///     The error code.
    /// </param>
    /// <param name="memberName">
    ///     The member name.
    /// </param>
    public ValidationError(string error, string? code = null, string? memberName = null) {
        Error = Throws.When.NullOrWhiteSpace(error);
        Code = code ?? string.Empty;
        MemberName = memberName ?? string.Empty;
    }
}