using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IProducer"/> for RabbitMQ producers.
/// </summary>
public sealed class Producer : IProducer {
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<Producer> _logger;

    // We are responsible for disposing the channel,
    // so we keep a reference to it.
    private IChannel? _channel;
    private bool _disposed;

    /// <inheritdoc />
    public string Topic { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Producer"/> class.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="channel">The channel.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="logger">The logger.</param>
    public Producer(string topic, IChannel channel, TimeProvider timeProvider, ILogger<Producer> logger) {
        Topic = Guard.Against.Null(topic);

        _channel = Guard.Against.Null(channel);
        _timeProvider = Guard.Against.Null(timeProvider);
        _logger = Guard.Against.Null(logger);
    }

    ~Producer() {
        Dispose(false);
    }

    /// <inheritdoc />
    public async Task ProduceAsync(object message, Parameters parameters, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        var properties = parameters.CreateBasicProperties();
        var buffer = CreateEnvelope(message, properties).GetBuffer();

        try {
            // if we don't have any routing key
            var routingKeys = parameters.GetRoutingKeys();
            if (routingKeys.Length == 0) {
                // to topic will be our routing key.
                routingKeys = [Topic];
            }

            foreach (var routingKey in routingKeys) {
                await GetChannel().BasicPublishAsync(parameters.GetExchangeName(),
                                       routingKey,
                                       parameters.GetMandatory(),
                                       properties,
                                       buffer,
                                       cancellationToken)
                                  .ConfigureAwait(false);
            }
        }
        catch (Exception ex) { _logger.UnhandledErrorWhileProducingMessage(ex); }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    private IChannel GetChannel() {
        if (_channel is null) {
            throw new InvalidOperationException("Channel is not available.");
        }

        return _channel;
    }

    private Envelope CreateEnvelope(object message, BasicProperties properties) {
        return new Envelope(message,
            properties.MessageId,
            properties.CorrelationId,
            _timeProvider.GetUtcNow());
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }
        if (disposing) {
            _channel?.Dispose();
        }

        _channel = null;
        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_channel is not null) {
            await _channel.CloseAsync(Constants.ReplySuccess, "Publisher finished work.")
                          .ConfigureAwait(false);

            await _channel.DisposeAsync()
                          .ConfigureAwait(false);
        }
    }
}