namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Exception thrown when a user is not found in the system.
/// </summary>
public class UserNotFoundException : Exception {
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException"/> class
    /// </summary>
    public UserNotFoundException()
        : this("User not found.") { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException"/> class
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    public UserNotFoundException(string message)
        : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException"/> class
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception.
    /// </param>
    public UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
