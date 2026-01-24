namespace Nameless.Bootstrap;

/// <summary>
///     Defines a contract for performing application startup or initialization
///     logic asynchronously.
/// </summary>
public interface IBootstrapper {
    /// <summary>
    ///     Asynchronously executes the bootstrap operation.
    /// </summary>
    /// <param name="context">
    ///     The context object that provides data and state information for
    ///     the steps execution.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous execution operation.
    /// </returns>
    Task ExecuteAsync(FlowContext context, CancellationToken cancellationToken);
}