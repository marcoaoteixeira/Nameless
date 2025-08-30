namespace Nameless.Mediator.Streams;

/// <summary>
///     Defines a stream requests.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IStream<out TResponse>;