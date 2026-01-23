namespace Nameless.Bootstrap;

/// <summary>
///     Represents errors that occur during the execution of the bootstrapper
///     process, providing information about the last executed step.
/// </summary>
public sealed class BootstrappingException : Exception {
    private const string MESSAGE_PATTERN = "An error occurred while executing the bootstrapper. The last executed step: {0}";

    /// <summary>
    ///     Gets the name of the last executed step.
    /// </summary>
    public string Step { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrappingException"/> class.
    /// </summary>
    /// <param name="step">
    ///     The name of the bootstrap step during which the exception occurred.
    /// </param>
    public BootstrappingException(string step)
        : base(string.Format(MESSAGE_PATTERN, step)) { Step = step; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrappingException"/> class.
    /// </summary>
    /// <param name="step">
    ///     The name of the bootstrap step during which the exception occurred.
    /// </param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception.
    /// </param>
    public BootstrappingException(string step, Exception innerException)
        : base(string.Format(MESSAGE_PATTERN, step), innerException) { Step = step; }
}