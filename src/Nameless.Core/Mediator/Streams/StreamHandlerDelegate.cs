namespace Nameless.Mediator.Streams;

/// <summary>
///     Represents an async continuation for the next task
///     to execute in the pipeline.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
/// <returns>
///     An <see cref="IAsyncEnumerable{T}" /> representing the stream of data.
/// </returns>
public delegate IAsyncEnumerable<TResponse> StreamHandlerDelegate<out TResponse>();