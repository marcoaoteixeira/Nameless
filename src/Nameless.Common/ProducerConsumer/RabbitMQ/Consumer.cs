using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.Resilience;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Abstract base class for RabbitMQ consumers that handles channel setup, message
///     delivery, acknowledgement, and retry logic as a hosted background service.
/// </summary>
/// <typeparam name="TMessage">The message type this consumer processes.</typeparam>
public abstract class Consumer<TMessage> : IConsumer<TMessage>, IHostedService, IDisposable, IAsyncDisposable {
    private readonly IChannelFactory _channelFactory;
    private readonly IMessageSerializer _serializer;
    private readonly IRetryPipelineFactory _retryPipelineFactory;
    private readonly ILogger<Consumer<TMessage>> _logger;

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

    /// <summary>
    ///     Gets the optional retry policy configuration for failed message processing.
    ///     Returns <see langword="null"/> by default (no retries).
    /// </summary>
    public virtual RetryPolicyConfiguration? RetryPolicy => null;

    private IRetryPipeline Retry => _retry.Value;

    private string ConsumerTag {
        get => field ??= string.IsNullOrWhiteSpace(Name)
            ? $"{typeof(TMessage).Name}_{Guid.CreateVersion7():N}"
            : Name;
    }

    /// <summary>
    ///     Initializes a new <see cref="Consumer{TMessage}"/>.
    /// </summary>
    /// <param name="channelFactory">Factory used to create the RabbitMQ channel.</param>
    /// <param name="serializer">Serializer for deserializing incoming messages.</param>
    /// <param name="retryPipelineFactory">Factory for creating the retry pipeline.</param>
    /// <param name="logger">Logger for this consumer instance.</param>
    protected Consumer(IChannelFactory channelFactory, IMessageSerializer serializer, IRetryPipelineFactory retryPipelineFactory, ILogger<Consumer<TMessage>> logger) {
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
    ///     Processes the deserialized <paramref name="message"/> received from RabbitMQ.
    /// </summary>
    /// <param name="message">The deserialized message payload.</param>
    /// <param name="context">Contextual metadata about the delivery (headers, correlation ID, etc.).</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous consume operation.</returns>
    public abstract Task ConsumeAsync(TMessage message, ConsumerContext context, CancellationToken cancellationToken);

    async Task IHostedService.StartAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        _channel = await _channelFactory
                         .CreateAsync(Topic, cancellationToken)
                         .SkipContextSync();

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
            Constants.ReplySuccess,
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
        var context = args.BasicProperties.ToConsumerContext();
        var message = await _serializer.DeserializeAsync<TMessage>(
            args.Body.ToArray(),
            context,
            args.CancellationToken
        ).SkipContextSync();

        try {
            await Retry.ExecuteAsync(
                async token => await ConsumeAsync(message, context, token).SkipContextSync(),
                args.CancellationToken
            );

            await PositiveAckAsync(
                args,
                args.CancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch {
            await NegativeAckAsync(
                args,
                args.CancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);

            throw;
        }
    }

    private ValueTask PositiveAckAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken) {
        if (_channel is null) { return ValueTask.CompletedTask; }

        return _channel.BasicAckAsync(
            args.DeliveryTag,
            multiple: false,
            cancellationToken
        );
    }

    private ValueTask NegativeAckAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken) {
        if (_channel is null) { return ValueTask.CompletedTask; }

        return _channel.BasicNackAsync(
            args.DeliveryTag,
            multiple: false,
            requeue: true,
            cancellationToken
        );
    }

    private IRetryPipeline CreateRetryPipeline() {
        if (RetryPolicy is null) { return RetryPipeline.Empty; }

        var configuration = RetryPolicy with { Tag = ConsumerTag };

        return _retryPipelineFactory.Create(configuration);
    }
}
