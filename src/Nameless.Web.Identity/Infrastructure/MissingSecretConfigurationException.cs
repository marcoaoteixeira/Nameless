namespace Nameless.Web.IdentityModel.Jwt;

/// <summary>
///     Exception thrown when the JSON Web Token (JWT) secret is not configured.
/// </summary>
public class MissingSecretConfigurationException : Exception {
    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingSecretConfigurationException"/> class.
    /// </summary>
    public MissingSecretConfigurationException()
        : this("The JSON Web Token (JWT) secret is not configured. Please ensure it is specified in the application settings.") { }

    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingSecretConfigurationException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    public MissingSecretConfigurationException(string message)
        : base(message) { }

    /// <summary>
    ///     Initializes a new instance
    ///     <see cref="MissingSecretConfigurationException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception, or a
    ///     <see langword="null"/> reference if no inner exception is
    ///     specified.
    /// </param>
    public MissingSecretConfigurationException(string message, Exception inner)
        : base(message, inner) { }
}
