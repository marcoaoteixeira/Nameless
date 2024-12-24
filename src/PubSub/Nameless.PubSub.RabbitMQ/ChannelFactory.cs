using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.PubSub.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

public sealed class ChannelFactory : IChannelFactory, IDisposable {
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<ChannelFactory> _logger;
    private readonly IOptions<RabbitMQOptions> _options;

    private SemaphoreSlim? _semaphore;
    private bool _disposed;

    public ChannelFactory(IConnectionManager connectionManager,
                          ILogger<ChannelFactory> logger,
                          IOptions<RabbitMQOptions> options) {
        _connectionManager = Prevent.Argument.Null(connectionManager);
        _logger = Prevent.Argument.Null(logger);
        _options = Prevent.Argument.Null(options);
    }

    ~ChannelFactory() {
        Dispose(disposing: false);
    }

    public async Task<IChannel> CreateChannelAsync(string exchangeName,
                                                   CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);
        try {
            var connection = await _connectionManager.GetConnectionAsync(cancellationToken)
                                                     .ConfigureAwait(continueOnCapturedContext: false);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken)
                                          .ConfigureAwait(continueOnCapturedContext: false);

            await ConfigureChannelAsync(channel, exchangeName, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);

            return channel;
        } finally { GetSemaphoreSlim().Release(); }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(nameof(ChannelFactory));
        }
#endif
    }

    private async Task ConfigureChannelAsync(IChannel channel, string exchangeName, CancellationToken cancellationToken) {
        // get the exchange object from options
        var exchange = _options.Value
                               .Exchanges
                               .SingleOrDefault(exchange => string.Equals(exchange.Name,
                                                                          exchangeName,
                                                                          StringComparison.Ordinal));

        // if the exchange object was not found, log and return.
        if (exchange is null) {
            _logger.MissingExchange(exchangeName);

            return;
        }

        // exchange found. let's declare our exchange
        await channel.ExchangeDeclareAsync(exchange: exchange.Name,
                                           type: exchange.Type.GetDescription(),
                                           durable: exchange.Durable,
                                           autoDelete: exchange.AutoDelete,
                                           arguments: exchange.Arguments,
                                           cancellationToken: cancellationToken)
                     .ConfigureAwait(continueOnCapturedContext: false);

        foreach (var queue in exchange.Queues) {
            // let's declare our queues
            await channel.QueueDeclareAsync(queue: queue.Name,
                                            durable: queue.Durable,
                                            exclusive: queue.Exclusive,
                                            autoDelete: queue.AutoDelete,
                                            arguments: queue.Arguments,
                                            cancellationToken: cancellationToken)
                         .ConfigureAwait(continueOnCapturedContext: false);

            // let's declare our bindings
            foreach (var binding in queue.Bindings) {
                await channel.QueueBindAsync(queue: queue.Name,
                                             exchange: exchange.Name,
                                             routingKey: binding.RoutingKey,
                                             arguments: binding.Arguments,
                                             cancellationToken: cancellationToken)
                             .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }

    private SemaphoreSlim GetSemaphoreSlim()
        => _semaphore ??= new SemaphoreSlim(initialCount: 1, maxCount: 1);
}