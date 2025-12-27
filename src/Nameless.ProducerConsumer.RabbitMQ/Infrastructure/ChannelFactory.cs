using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelFactory"/> for creating RabbitMQ channels. 
/// </summary>
public sealed class ChannelFactory : IChannelFactory {
    private readonly IConnectionManager _connectionManager;

    /// <summary>
    /// Initializes a new instance of <see cref="ChannelFactory"/>.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    public ChannelFactory(IConnectionManager connectionManager) {
        _connectionManager = Guard.Against.Null(connectionManager);
    }

    /// <inheritdoc />
    public async Task<IChannel> CreateAsync(CancellationToken cancellationToken) {
        var connection = await _connectionManager.GetConnectionAsync(cancellationToken)
                                                 .ConfigureAwait(continueOnCapturedContext: false);

        return await connection.CreateChannelAsync(cancellationToken: cancellationToken)
                               .ConfigureAwait(continueOnCapturedContext: false);
    }
}