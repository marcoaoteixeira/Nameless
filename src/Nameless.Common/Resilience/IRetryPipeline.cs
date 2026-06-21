namespace Nameless.Resilience;

/// <summary>
///     Represents a retry pipeline.
/// </summary>
public interface IRetryPipeline {
    /// <summary>
    ///     Gets the pipeline identifier.
    /// </summary>
    string? Tag { get; }

    /// <summary>
    ///     Executes the <paramref name="operation"/> using the pipeline
    ///     policy.
    /// </summary>
    /// <param name="operation">
    ///     The operation to execute.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token that will be passed down to the operation.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    ValueTask ExecuteAsync(Func<CancellationToken, ValueTask> operation, CancellationToken cancellationToken);

    /// <summary>
    ///     Executes the <paramref name="operation"/> using the pipeline
    ///     policy.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the operation result.
    /// </typeparam>
    /// <param name="operation">
    ///     The operation to execute.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token that will be passed down to the operation.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.
    ///     Where the result is the operation result.
    /// </returns>
    ValueTask<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> operation, CancellationToken cancellationToken);
}
