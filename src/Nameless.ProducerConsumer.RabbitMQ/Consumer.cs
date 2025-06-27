using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IConsumer{TMessage}" /> for RabbitMQ.
/// </summary>
/// <typeparam name="TMessage">Type of the output.</typeparam>
public sealed class Consumer<TMessage> : IConsumer<TMessage>
    where TMessage : notnull {
    private readonly ILogger<Consumer<TMessage>> _logger;

    // we are 
    private IChannel? _channel;
    private bool _disposed;
    private MessageHandlerDelegate<TMessage>? _handler;
    private AsyncEventingBasicConsumer? _consumer;
    private CancellationTokenSource? _cancellationTokenSource;
    private string _consumerTag = string.Empty;
    private bool _started;

    /// <inheritdoc />
    public string Topic { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Consumer{TMessage}" /> class.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="channel">The channel.</param>
    /// <param name="logger">The logger.</param>
    public Consumer(string topic, IChannel channel, ILogger<Consumer<TMessage>> logger) {
        Topic = Prevent.Argument.Null(topic);

        _channel = Prevent.Argument.Null(channel);
        _logger = Prevent.Argument.Null(logger);
    }

    ~Consumer() {
        Dispose(disposing: false);
    }

    public async Task StartAsync(MessageHandlerDelegate<TMessage> handler, Parameters parameters, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        if (_started) {
            _logger.ConsumerAlreadyStarted();

            return;
        }

        _handler = Prevent.Argument.Null(handler);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var channel = GetChannel();

        // creates the consumer for the channel
        _consumer = new AsyncEventingBasicConsumer(channel);

        // monitor the channel for shutdown events
        _consumer.ShutdownAsync += ConsumerShutdownAsync;

        // register the consumer with the channel
        _consumer.ReceivedAsync += ConsumerReceivedAsync;

        // startup the consumer
        _consumerTag = Guid.NewGuid().ToString(format: "N");
        var startupStatus = await channel.BasicConsumeAsync(Topic,
                                              parameters.GetAutoAck(),
                                              _consumerTag,
                                              _consumer,
                                              cancellationToken)
                                         .ConfigureAwait(false);

        _logger.ConsumerStarted(startupStatus, _consumerTag, parameters);
        _started = true;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private MessageHandlerDelegate<TMessage> GetMessageHandler() {
        if (_handler is null) {
            throw new InvalidOperationException(message: $"Consumer '{_consumerTag}' output handler is not available.");
        }

        return _handler;
    }

    private IChannel GetChannel() {
        if (_channel is null) {
            throw new InvalidOperationException(message: $"Consumer '{_consumerTag}' channel is not available.");
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
                          .ConfigureAwait(false);

            await _channel.DisposeAsync()
                          .ConfigureAwait(false);
        }

        _cancellationTokenSource?.Dispose();
    }

    private Task ConsumerShutdownAsync(object sender, ShutdownEventArgs shutdownEventArgs) {
        _logger.ConsumerShutdown(_consumerTag, shutdownEventArgs.ReplyText);

        return Task.CompletedTask;
    }

    private Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs basicDeliverEventArgs) {
        if (!TryDeserializeEnvelope(basicDeliverEventArgs, out var envelope)) {
            return Task.CompletedTask;
        }

        _logger.EnvelopeReceived(envelope);

        if (!TryDeserializeMessage(envelope, out var message)) {
            return Task.CompletedTask;
        }

        try { return GetMessageHandler().Invoke(message, GetCancellationTokenSource().Token); }
        catch (Exception ex) { _logger.MessageHandlerThrownException(_consumerTag, ex); }

        return Task.CompletedTask;
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs basicDeliverEventArgs, [NotNullWhen(returnValue: true)] out Envelope? output) {
        output = null;

        // Our "Envelope" is an array of bytes that contains
        // the serialized (JSON) version of the Envelope instance.
        try { output = JsonSerializer.Deserialize<Envelope>(basicDeliverEventArgs.Body.ToArray()); }
        catch (Exception ex) {
            _logger.EnvelopeDeserializationError(_consumerTag, ex);

            return false;
        }

        if (output is null) {
            // Deserialization didn't throw exception, but failed.
            // We were not able to retrieve the output for some reason.
            _logger.EnvelopeDeserializationFailure(basicDeliverEventArgs);
        }

        return output is not null;
    }

    private bool TryDeserializeMessage(Envelope envelope, [NotNullWhen(returnValue: true)] out TMessage? output) {
        // Since we are serialize to JSON before publishing the envelope,
        // we don't have the type of the message here nor do we care.
        // What we care is that the property "Message" is a JSON object
        // now, and we need to deserialize it to the type that we want.
        output = ((JsonNode)envelope.Message).Deserialize<TMessage>();

        if (output is null) {
            _logger.UnableDeserializeMessage(typeof(TMessage));
        }

        return output is not null;
    }
}