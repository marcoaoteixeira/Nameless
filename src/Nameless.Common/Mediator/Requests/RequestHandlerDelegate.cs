namespace Nameless.Mediator.Requests;

/// <summary>
///     Represents an async continuation for the next task
///     to execute in the pipeline.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
/// <param name="cancellationToken">
///     The cancellation token.
/// </param>
/// <returns>
///     A <see cref="Task{TResult}" /> representing the action
///     asynchronous operation, where <typeparamref name="TResponse"/>
///     is the task result.
/// </returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

/// <summary>
///     Represents an async continuation for the next task
///     to execute in the pipeline of a request without a response.
/// </summary>
/// <param name="cancellationToken">
///     The cancellation token.
/// </param>
/// <returns>
///     A <see cref="Task" /> representing the action asynchronous
///     operation.
/// </returns>
public delegate Task RequestHandlerDelegate(CancellationToken cancellationToken);
