namespace Nameless.ProducerConsumer;

/// <summary>
///     Defines a factory for creating producers.
/// </summary>
public interface IProducerFactory {
    /// <summary>
    ///     Creates a producer for the specified topic with the given arguments.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation,
    ///     containing the created <see cref="IProducer"/> instance as its result.
    /// </returns>
    Task<IProducer> CreateAsync(string topic, Args args, CancellationToken cancellationToken);
}