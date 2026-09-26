using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Represents a RabbitMQ connection manager that provides access to RabbitMQ connections.
/// </summary>
public interface IConnectionManager {
    /// <summary>
    /// Asynchronously retrieves a RabbitMQ connection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IConnection"/> instance.
    /// </returns>
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);
}