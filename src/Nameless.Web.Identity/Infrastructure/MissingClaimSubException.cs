namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Exception thrown when the 'sub' claim is missing.
/// </summary>
public class MissingClaimSubException : Exception {
    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingClaimSubException"/> class.
    /// </summary>
    public MissingClaimSubException()
        : this($"The required claim '{MS_JwtRegisteredClaimNames.Sub}' is missing.") {
    }

    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingClaimSubException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    public MissingClaimSubException(string message) : base(message) { }

    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingClaimSubException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception, or a
    ///     <see langword="null"/> reference if no inner exception is
    ///     specified.
    /// </param>
    public MissingClaimSubException(string message, Exception inner) : base(message, inner) { }
}