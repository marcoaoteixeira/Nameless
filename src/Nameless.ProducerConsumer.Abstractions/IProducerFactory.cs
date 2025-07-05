namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a producer factory.
/// </summary>
public interface IProducerFactory {
    /// <summary>
    ///     Creates a producer and associated it to the specified topic.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="parameters">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation.
    ///     The result of the task is a new <see cref="IProducer"/> instance.
    /// </returns>
    Task<IProducer> CreateAsync(string topic, Parameters parameters, CancellationToken cancellationToken);
}