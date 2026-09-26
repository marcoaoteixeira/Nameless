using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Resilience;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Abstract base class for RabbitMQ consumers that handles channel setup, message
///     delivery, acknowledgement, and retry logic as a hosted background service.
/// </summary>
/// <typeparam name="T">
///     The message content type this consumer processes.
/// </typeparam>
public abstract class Consumer<T> : IConsumer<T>, IHostedService, IDisposable, IAsyncDisposable {
    private readonly ConsumerOptions _options;
    private readonly IChannelFactory _channelFactory;
    private readonly IMessageSerializer _serializer;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly ILogger _logger;

    private readonly Lazy<IRetryPipeline> _retry;

    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private bool _disposed;

    /// <summary>
    ///     Gets the consumer name used as the RabbitMQ consumer tag.
    ///     When empty or whitespace, a unique name is generated automatically.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Gets the queue/topic name this consumer subscribes to.
    /// </summary>
    public abstract string Topic { get; }

    private IRetryPipeline Retry => _retry.Value;

    private string ConsumerTag {
        get => field ??= string.IsNullOrWhiteSpace(Name)
            ? $"{typeof(T).Name}_{Guid.CreateVersion7():N}"
            : Name;
    }

    /// <summary>
    ///     Initializes a new <see cref="Consumer{TMessage}"/>.
    /// </summary>
    /// <param name="channelFactory">Factory used to create the RabbitMQ channel.</param>
    /// <param name="serializer">Serializer for deserializing incoming messages.</param>
    /// <param name="retryPipelineFactory">Factory for creating the retry pipeline.</param>
    /// <param name="options">The consumer options.</param>
    /// <param name="logger">Logger for this consumer instance.</param>
    protected Consumer(IChannelFactory channelFactory, IMessageSerializer serializer, IRetryPipelineFactory retryPipelineFactory, ConsumerOptions options, ILogger logger) {
        _options = options;
        _channelFactory = channelFactory;
        _serializer = serializer;
        _retryPipelineFactory = retryPipelineFactory;
        _logger = logger;

        _retry = new Lazy<IRetryPipeline>(CreateRetryPipeline);
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~Consumer() {
        Dispose(disposing: false);
    }

    /// <summary>
    ///     Processes the deserialized <paramref name="value"/> received from RabbitMQ.
    /// </summary>
    /// <param name="value">The deserialized message payload.</param>
    /// <param name="context">Contextual metadata about the delivery (headers, correlation ID, etc.).</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous consume operation.</returns>
    public abstract Task ConsumeAsync(T value, ConsumerContext context, CancellationToken cancellationToken);

    async Task IHostedService.StartAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        _channel = await _channelFactory.CreateAsync(Topic, cancellationToken).SkipContextSync();

        // creates the consumer for the channel
        _consumer = new AsyncEventingBasicConsumer(_channel);

        // monitor the channel for shutdown events
        _consumer.ShutdownAsync += ConsumerShutdownAsync;

        // register the consumer with the channel
        _consumer.ReceivedAsync += ConsumerReceivedAsync;

        // startup the consumer
        var reply = await _channel.BasicConsumeAsync(
            queue: Topic,
            autoAck: false,
            ConsumerTag,
            _consumer,
            cancellationToken
        ).SkipContextSync();

        Log.ConsumerStarted(_logger, ConsumerTag, reply);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        if (_channel is null) { return Task.CompletedTask; }

        _consumer?.ShutdownAsync -= ConsumerShutdownAsync;
        _consumer?.ReceivedAsync -= ConsumerReceivedAsync;

        return _channel.CloseAsync(
            RabbitConstants.ReplySuccess,
            replyText: "Consumer work finished.",
            cancellationToken
        );
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases managed and unmanaged resources held by this consumer.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true"/> when called from <see cref="Dispose()"/>;
    ///     <see langword="false"/> when called from the finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _channel?.Dispose();
        }

        _consumer?.ShutdownAsync -= ConsumerShutdownAsync;
        _consumer?.ReceivedAsync -= ConsumerReceivedAsync;

        _channel = null;
        _consumer = null;
        _disposed = true;
    }

    /// <summary>
    ///     Performs the asynchronous portion of resource cleanup, disposing the channel asynchronously.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore() {
        if (_channel is not null) {
            await _channel.DisposeAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private Task ConsumerShutdownAsync(object sender, ShutdownEventArgs shutdownEventArgs) {
        Log.ConsumerShutdown(_logger, ConsumerTag, shutdownEventArgs.ReplyText);

        return Task.CompletedTask;
    }

    private async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs args) {
        var message = _serializer.Deserialize<T>(args.Body.ToArray());
        var context = args.BasicProperties.ToConsumerContext();

        context.MessageId = message.Header.MessageID;
        context.CorrelationId = message.Header.CorrelationID;
        context.Timestamp = new AmqpTimestamp(message.Header.Timestamp);
        context.DeliveryTag = args.DeliveryTag;

        try {
            await Retry.ExecuteAsync(
                async token => await ConsumeAsync(message.Content, context, token).SkipContextSync(),
                args.CancellationToken // token from StartAsync
            );

            await PositiveAckAsync(args).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (OperationCanceledException) {
            await NegativeAckAsync(args, requeue: true).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) {
            var requeue = ex is NoRetryException { Requeue: true };

            await NegativeAckAsync(args, requeue).ConfigureAwait(continueOnCapturedContext: false);
            
            throw;
        }
    }

    private async ValueTask PositiveAckAsync(BasicDeliverEventArgs args) {
        if (_channel is null) { return; }
        
        try {
            using var cts = new CancellationTokenSource(Constants.MessageConfirmationTimeout);
            await _channel.BasicAckAsync(args.DeliveryTag, multiple: false, cts.Token)
                          .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (OperationCanceledException) { CommonLog.Warning(_logger, $"Timeout from ACK call | Delivery tag: {args.DeliveryTag}", GetType().Tag); }
        catch (Exception ex) { CommonLog.Error(_logger, $"{ex.Message} | Delivery tag: {args.DeliveryTag}", ex, GetType().Tag); throw; }
    }

    private async ValueTask NegativeAckAsync(BasicDeliverEventArgs args, bool requeue) {
        if (_channel is null) { return; }
        
        try {
            using var cts = new CancellationTokenSource(Constants.MessageConfirmationTimeout);
            await _channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue, cts.Token)
                          .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (OperationCanceledException) { CommonLog.Warning(_logger, $"Timeout from NACK call | Delivery tag: {args.DeliveryTag}", GetType().Tag); }
        catch (Exception ex) { CommonLog.Error(_logger, $"{ex.Message} | Delivery tag: {args.DeliveryTag}", ex, GetType().Tag); throw; }
    }

    private IRetryPipeline CreateRetryPipeline() {
        var policy = _options.RetryPolicy ?? new RetryPolicyOptions();
        var config = policy.CreateConfiguration(
            ConsumerTag,
            onRetry: (_, _, _, _) => { },
            onRetryException: ex => ex is not NoRetryException
        );

        return _retryPipelineFactory.Create(config);
    }
}
