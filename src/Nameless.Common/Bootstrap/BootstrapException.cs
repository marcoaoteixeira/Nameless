using Nameless.Bootstrap.Execution;

namespace Nameless.Bootstrap;

/// <summary>
///     Represents errors that occur during the execution of the bootstrapper
///     process, providing information about the last executed step.
/// </summary>
public sealed class BootstrapException : Exception {
    private const string DEFAULT_MESSAGE = "An error occurred while executing the bootstrapper";

    /// <summary>
    ///     Gets the results from the Bootstrap execution
    /// </summary>
    public StepExecutionResult[] Results { get; } = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    public BootstrapException()
        : base(DEFAULT_MESSAGE) { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The exception message.
    /// </param>
    public BootstrapException(string message)
        : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="results">
    ///     The steps execution result.
    /// </param>
    public BootstrapException(IEnumerable<StepExecutionResult> results)
        : base(DEFAULT_MESSAGE) {
        Results = [.. results];
    }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="results">
    ///     The steps execution result.
    /// </param>
    public BootstrapException(string message, IEnumerable<StepExecutionResult> results)
        : base(message) {
        Results = [.. results];
    }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="BootstrapException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="results">
    ///     The steps execution result.
    /// </param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception.
    /// </param>
    public BootstrapException(string message, IEnumerable<StepExecutionResult> results, Exception innerException)
        : base(message, innerException) {
        Results = [.. results];
    }
}