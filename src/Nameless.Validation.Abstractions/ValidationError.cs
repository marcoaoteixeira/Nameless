namespace Nameless.Validation;

/// <summary>
///     Represents a validation error entry.
/// </summary>
public sealed record ValidationError {
    /// <summary>
    ///     Gets the validation error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the validation error code.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the member responsible for the validation error.
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationError"/>.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="memberName">The member name.</param>
    public ValidationError(string message, string? errorCode = null, string? memberName = null) {
        Message = Prevent.Argument.NullOrWhiteSpace(message);
        ErrorCode = errorCode ?? string.Empty;
        MemberName = memberName ?? string.Empty;
    }
}