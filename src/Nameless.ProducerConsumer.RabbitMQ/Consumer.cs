using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IConsumer" /> for RabbitMQ.
/// </summary>
public sealed class Consumer : IConsumer {
    private readonly ILogger<Consumer> _logger;
    private CancellationTokenSource? _cancellationTokenSource;

    private IChannel? _channel;
    private bool _disposed;
    private MessageHandlerDelegate? _handler;
    private AsyncEventingBasicConsumer? _consumer;
    private bool _started;

    /// <inheritdoc />
    public string Topic { get; }

    private string ConsumerTag { get; set; } = string.Empty;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Consumer" /> class.
    /// </summary>
    /// <param name="channel">The channel.</param>
    /// <param name="topic">The topic.</param>
    /// <param name="logger">The logger.</param>
    public Consumer(IChannel channel, string topic, ILogger<Consumer> logger) {
        _channel = Prevent.Argument.Null(channel);
        _logger = Prevent.Argument.Null(logger);

        Topic = Prevent.Argument.Null(topic);
    }

    ~Consumer() {
        Dispose(disposing: false);
    }

    public async Task StartAsync(MessageHandlerDelegate handler, Args args, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        _handler = Prevent.Argument.Null(handler);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (_started) {
            _logger.ConsumerAlreadyStarted();

            return;
        }

        var channel = GetChannel();

        // creates the consumer for the channel
        _consumer = new AsyncEventingBasicConsumer(channel);

        // monitor the channel for shutdown events
        _consumer.ShutdownAsync += ConsumerShutdownAsync;

        // register the consumer with the channel
        _consumer.ReceivedAsync += ConsumerReceivedAsync;

        // startup the consumer
        ConsumerTag = Guid.NewGuid().ToString(format: "N");
        var startupStatus = await channel.BasicConsumeAsync(args.GetQueueName(),
                                              args.GetAutoAck(),
                                              ConsumerTag,
                                              _consumer,
                                              cancellationToken)
                                         .ConfigureAwait(continueOnCapturedContext: false);

        _logger.ConsumerStarted(startupStatus, ConsumerTag, args);
        _started = true;
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

    private MessageHandlerDelegate GetMessageHandler() {
        if (_handler is null) {
            throw new InvalidOperationException(message: $"Consumer '{ConsumerTag}' message handler is not available.");
        }

        return _handler;
    }

    private IChannel GetChannel() {
        if (_channel is null) {
            throw new InvalidOperationException(message: $"Consumer '{ConsumerTag}' channel is not available.");
        }

        return _channel;
    }

    private CancellationTokenSource GetCancellationTokenSource() {
        if (_cancellationTokenSource is null) {
            throw new InvalidOperationException(message: "Cancellation token source is not available.");
        }

        return _cancellationTokenSource;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _channel?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        _channel = null;
        _cancellationTokenSource = null;
        _handler = null;
        _consumer = null;
        _started = false;

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_channel is not null) {
            await _channel.CloseAsync(Constants.ReplySuccess, replyText: "Consumer work finished.",
                               CancellationToken.None)
                          .ConfigureAwait(continueOnCapturedContext: false);

            await _channel.DisposeAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);
        }

        _cancellationTokenSource?.Dispose();
    }

    private Task ConsumerShutdownAsync(object sender, ShutdownEventArgs shutdownEventArgs) {
        _logger.ConsumerShutdown(ConsumerTag, shutdownEventArgs.ReplyText);

        return Task.CompletedTask;
    }

    private Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs basicDeliverEventArgs) {
        if (!TryDeserializeEnvelope(basicDeliverEventArgs, out var envelope)) {
            return Task.CompletedTask;
        }

        try { return GetMessageHandler().Invoke(envelope.Message, GetCancellationTokenSource().Token); }
        catch (Exception ex) { _logger.MessageHandlerThrownException(ConsumerTag, ex); }

        return Task.CompletedTask;
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs basicDeliverEventArgs, [NotNullWhen(returnValue: true)] out Envelope? envelope) {
        envelope = null;

        // Our "Envelope" is an array of bytes that contains
        // the serialized (JSON) version of the Envelope instance.
        try { envelope = JsonSerializer.Deserialize<Envelope>(basicDeliverEventArgs.Body.ToArray()); }
        catch (Exception ex) {
            _logger.EnvelopeDeserializationError(ConsumerTag, ex);

            return false;
        }

        if (envelope is null) {
            // Deserialization didn't throw exception, but failed.
            // We were not able to retrieve the envelope for some reason.
            _logger.EnvelopeDeserializationFailure(basicDeliverEventArgs);
        }

        return envelope is not null;
    }
}