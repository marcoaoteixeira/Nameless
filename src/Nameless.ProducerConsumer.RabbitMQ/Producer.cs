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

    /// <summary>
    ///     Initializes a new instance of the <see cref="Producer"/> class.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="topic">The topic.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="logger">The logger.</param>
    public Producer(IChannel channel, string topic, TimeProvider timeProvider, ILogger<Producer> logger) {
        _channel = Prevent.Argument.Null(channel);
        _timeProvider = Prevent.Argument.Null(timeProvider);
        _logger = Prevent.Argument.Null(logger);

        Topic = Prevent.Argument.Null(topic);
    }

    ~Producer() {
        Dispose(disposing: false);
    }

    public string Topic { get; }

    public async Task ProduceAsync(object message, Args args, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        var properties = CreateProperties(args);
        var buffer = CreateEnvelope(message, properties).GetBuffer();

        try {
            foreach (var routingKey in args.GetRoutingKeys()) {
                await GetChannel().BasicPublishAsync(Topic,
                                       routingKey,
                                       args.GetMandatory(),
                                       properties,
                                       buffer,
                                       cancellationToken)
                                  .ConfigureAwait(false);
            }
        }
        catch (Exception ex) { _logger.UnhandledErrorWhileProducingMessage(ex); }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private IChannel GetChannel() {
        if (_channel is null) {
            throw new InvalidOperationException("Channel is not available.");
        }

        return _channel;
    }

    private Envelope CreateEnvelope(object message, BasicProperties properties) {
        return new Envelope(message, properties.MessageId, properties.CorrelationId, _timeProvider.GetUtcNow());
    }

    private static BasicProperties CreateProperties(Args args) {
        return new BasicProperties().FillWith(args);
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
                          .ConfigureAwait(continueOnCapturedContext: false);

            await _channel.DisposeAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}