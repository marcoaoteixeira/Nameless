using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ.Contracts;

/// <summary>
/// Provides means to manage RabbitMQ connections.
/// </summary>
public interface IConnectionManager {
    Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken);
}