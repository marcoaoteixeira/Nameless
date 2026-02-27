namespace Nameless.Bootstrap.Infrastructure;

/// <summary>
///     Represents errors that occur during the execution of the bootstrapper
///     process, providing information about the last executed step.
/// </summary>
public sealed class BootstrapException : Exception {
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    public BootstrapException()
        : base(message: "An error occurred while executing the bootstrapper") { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    public BootstrapException(string message)
        : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception.
    /// </param>
    public BootstrapException(string message, Exception innerException)
        : base(message, innerException) { }
}