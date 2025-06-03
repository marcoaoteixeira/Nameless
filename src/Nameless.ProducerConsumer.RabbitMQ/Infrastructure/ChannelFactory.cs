using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IChannelFactory"/> for creating RabbitMQ channels. 
/// </summary>
public sealed class ChannelFactory : IChannelFactory, IDisposable {
    private readonly IConnectionManager _connectionManager;
    private readonly IChannelConfigurator _channelConfigurator;

    private SemaphoreSlim? _semaphore;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="ChannelFactory"/>.
    /// </summary>
    /// <param name="connectionManager">The connection manager.</param>
    /// <param name="channelConfigurator">The channel configurator.</param>
    public ChannelFactory(IConnectionManager connectionManager, IChannelConfigurator channelConfigurator) {
        _connectionManager = Prevent.Argument.Null(connectionManager);
        _channelConfigurator = Prevent.Argument.Null(channelConfigurator);
    }

    /// <inheritdoc />
    public async Task<IChannel> CreateAsync(string exchangeName, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);
        try {
            var connection = await _connectionManager.GetConnectionAsync(cancellationToken)
                                                     .ConfigureAwait(false);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken)
                                          .ConfigureAwait(false);

            await _channelConfigurator.ConfigureChannelAsync(channel, exchangeName, cancellationToken)
                                      .ConfigureAwait(false);

            return channel;
        }
        finally { GetSemaphoreSlim().Release(); }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ChannelFactory() {
        Dispose(false);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _semaphore?.Dispose();
        }

        _semaphore = null;

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private SemaphoreSlim GetSemaphoreSlim() {
        return _semaphore ??= new SemaphoreSlim(1, 1);
    }
}