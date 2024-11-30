using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// Provides means to manage RabbitMQ connections.
/// </summary>
public interface IConnectionManager {
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);
}