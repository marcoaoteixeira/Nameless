namespace Nameless.ProducerConsumer;

public interface IConsumerFactory {
    /// <summary>
    ///     Creates a consumer for the specified topic with the given arguments.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation,
    ///     containing the created <see cref="IConsumer"/> instance as its result.
    /// </returns>
    Task<IConsumer> CreateAsync(string topic, Args args, CancellationToken cancellationToken);
}