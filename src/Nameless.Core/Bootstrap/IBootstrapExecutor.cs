namespace Nameless.Bootstrap;

/// <summary>
///     Defines a contract for performing application startup or initialization
///     logic asynchronously.
/// </summary>
public interface IBootstrapExecutor {
    /// <summary>
    ///     Asynchronously executes the bootstrap operation.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous execution operation.
    /// </returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}