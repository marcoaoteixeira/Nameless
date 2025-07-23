namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Represents an exception that is thrown when an invalid refresh token
///     is encountered.
/// </summary>
public class InvalidRefreshTokenException : Exception {
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InvalidRefreshTokenException"/> class.
    /// </summary>
    public InvalidRefreshTokenException()
        : this("Invalid refresh token.") { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InvalidRefreshTokenException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    public InvalidRefreshTokenException(string message) : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="InvalidRefreshTokenException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    ///     The inner exception that is the cause of the current exception.
    /// </param>
    public InvalidRefreshTokenException(string message, Exception inner) : base(message, inner) { }
}
