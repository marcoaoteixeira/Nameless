namespace Nameless.Patterns.Mediator.Streams;

/// <summary>
/// The base interface for all stream requests.
/// </summary>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public interface IStream<out TResponse>;