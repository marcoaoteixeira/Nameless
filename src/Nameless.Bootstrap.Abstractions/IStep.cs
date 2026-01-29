namespace Nameless.Bootstrap;

/// <summary>
///     Represents a single step in the bootstrap process.
/// </summary>
public interface IStep {
    /// <summary>
    ///     Gets the name of the step.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Whether it should throw exception on error.
    /// </summary>
    bool ThrowOnError { get; }

    /// <summary>
    ///     Asynchronously executes the step logic using
    ///     the specified flow context.
    /// </summary>
    /// <param name="context">
    ///     The context object that provides data and state information for
    ///     the current step execution.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the asynchronous
    ///     operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}