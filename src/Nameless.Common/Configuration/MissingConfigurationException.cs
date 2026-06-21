namespace Nameless.Configuration;

/// <summary>
///     Exception thrown when required configuration is not present.
///     Use this exception to indicate a missing configuration section or
///     a missing key within a section.
/// </summary>
public sealed class MissingConfigurationException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="section">
    ///     The name of the configuration section that is missing.
    /// </param>
    public MissingConfigurationException(string section)
        : base($"Missing configuration section '{section}'") { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="section">
    ///     The name of the configuration section containing the missing
    ///     parameter.
    /// </param>
    /// <param name="key">
    ///     The name of the missing configuration key.
    /// </param>
    public MissingConfigurationException(string section, string key)
        : base($"Missing configuration key '{key}' in section '{section}'") { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="MissingConfigurationException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="inner">
    ///     The exception that is the cause of the current exception,
    ///     or <see langword="null"/>.
    /// </param>
    public MissingConfigurationException(string message, Exception inner)
        : base(message, inner) { }
}