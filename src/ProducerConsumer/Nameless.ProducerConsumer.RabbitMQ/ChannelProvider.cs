using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class ChannelProvider : IChannelProvider, IDisposable {
    private readonly IConnectionManager _connectionManager;
    private readonly IOptions<RabbitMQOptions> _options;
    
    private readonly List<string> _exchanges = [];

    private SemaphoreSlim? _semaphore;
    private bool _disposed;

    public ChannelProvider(IConnectionManager connectionManager,
                           IOptions<RabbitMQOptions> options) {
        _connectionManager = Prevent.Argument.Null(connectionManager);
        _options = Prevent.Argument.Null(options);
    }

    ~ChannelProvider() {
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

            if (ShouldConfigureExchange(exchangeName)) {
                await ConfigureExchangeAsync(channel,
                                             exchangeName,
                                             cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }

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
            throw new ObjectDisposedException(GetType().Name);
        }
#endif
    }

    private bool ShouldConfigureExchange(string exchangeName)
        => _exchanges.Contains(exchangeName, StringComparer.CurrentCulture);

    private void MarkExchangeAsConfigured(string exchangeName)
        => _exchanges.Add(exchangeName);

    private async Task ConfigureExchangeAsync(IChannel channel,
                                              string exchangeName,
                                              CancellationToken cancellationToken) {
        var exchange = _options.Value
                               .Exchanges
                               .SingleOrDefault(exchange => string.Equals(exchange.Name,
                                                                          exchangeName,
                                                                          StringComparison.CurrentCulture));

        if (exchange is null) { return; }

        // let's declare our exchange
        await DeclareExchangeAsync(channel,
                                   exchange,
                                   cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        foreach (var queue in exchange.Queues) {
            // let's declare our queue
            await DeclareQueueAsync(channel,
                                    queue,
                                    cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);

            // let's declare our bindings
            foreach (var binding in queue.Bindings) {
                await DeclareBindingAsync(channel,
                                          exchange,
                                          queue,
                                          binding,
                                          cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        MarkExchangeAsConfigured(exchangeName);
    }

    private static Task DeclareExchangeAsync(IChannel channel,
                                             ExchangeSettings exchange,
                                             CancellationToken cancellationToken)
        => channel.ExchangeDeclareAsync(exchange: exchange.Name,
                                        type: exchange.Type.GetDescription(),
                                        durable: exchange.Durable,
                                        autoDelete: exchange.AutoDelete,
                                        arguments: exchange.Arguments,
                                        cancellationToken: cancellationToken);

    private static Task<QueueDeclareOk> DeclareQueueAsync(IChannel channel,
                                                          QueueSettings queue,
                                                          CancellationToken cancellationToken)
        => channel.QueueDeclareAsync(queue: queue.Name,
                                     durable: queue.Durable,
                                     exclusive: queue.Exclusive,
                                     autoDelete: queue.AutoDelete,
                                     arguments: queue.Arguments,
                                     cancellationToken: cancellationToken);

    private static Task DeclareBindingAsync(IChannel channel,
                                            ExchangeSettings exchange,
                                            QueueSettings queue,
                                            BindingSettings binding,
                                            CancellationToken cancellationToken)
        => channel.QueueBindAsync(queue: queue.Name,
                                  exchange: exchange.Name,
                                  routingKey: binding.RoutingKey,
                                  arguments: binding.Arguments,
                                  cancellationToken: cancellationToken);

    private SemaphoreSlim GetSemaphoreSlim()
        => _semaphore ??= new SemaphoreSlim(initialCount: 1, maxCount: 1);
}