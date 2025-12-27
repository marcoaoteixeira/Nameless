namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a consumer factory.
/// </summary>
public interface IConsumerFactory {
    /// <summary>
    ///     Creates a consumer and associated it to the specified topic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    /// <param name="topic">The topic.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> representing the asynchronous operation.
    ///     The result of the task is a new <see cref="IConsumer{TMessage}"/> instance.
    /// </returns>
    Task<IConsumer<TMessage>> CreateAsync<TMessage>(string topic, Parameters parameters,
        CancellationToken cancellationToken)
        where TMessage : notnull;
}