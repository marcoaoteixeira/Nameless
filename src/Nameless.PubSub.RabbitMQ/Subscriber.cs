using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.PubSub.RabbitMQ.Infrastructure;
using Nameless.PubSub.RabbitMQ.Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.PubSub.RabbitMQ;

/// <summary>
/// Default implementation of <see cref="ISubscriber"/> for RabbitMQ.
/// </summary>
public sealed class Subscriber : ISubscriber, IDisposable {
    private readonly IChannelFactory _channelFactory;
    private readonly ILogger<Subscriber> _logger;

    private Dictionary<string, CacheEntry> Cache { get; } = [];
    private SemaphoreSlim? _semaphoreSlim;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscriber"/> class.
    /// </summary>
    /// <param name="channelFactory">The channel factory.</param>
    /// <param name="logger">The logger.</param>
    public Subscriber(IChannelFactory channelFactory, ILogger<Subscriber> logger) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     The parameter <paramref name="topic" /> is the RabbitMQ exchange name.
    ///     The queue name should be provided through the method <see cref="SubscriberArgsExtensions.SetQueueName" />
    ///     applied to the parameter <paramref name="args" /> before calling <see cref="SubscribeAsync" />.
    /// </remarks>
    public async Task<string> SubscribeAsync(string topic,
                                             MessageHandlerDelegate messageHandler,
                                             SubscriberArgs args,
                                             CancellationToken cancellationToken) {
        Prevent.Argument.Null(topic);
        Prevent.Argument.Null(messageHandler);

        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);
        try {
            var cacheKey = CreateCacheKey(topic, messageHandler);

            if (!Cache.ContainsKey(cacheKey)) {
                Cache[cacheKey] = await CreateCacheEntryAsync(topic,
                    messageHandler,
                    args,
                    cancellationToken);
            }

            return cacheKey;
        }
        finally { GetSemaphoreSlim().Release(); }
    }

    public async Task<bool> UnsubscribeAsync(string subscription,
                                             CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(subscription);

        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);
        try {
            if (Cache.Remove(subscription, out var cacheEntry)) {
                await cacheEntry.DisposeAsync().ConfigureAwait(false);
            }

            return true;
        }
        catch { return false; }
        finally { GetSemaphoreSlim().Release(); }
    }

    ~Subscriber() {
        Dispose(false);
    }

    private async Task<CacheEntry> CreateCacheEntryAsync(string topic,
                                                         MessageHandlerDelegate messageHandler,
                                                         SubscriberArgs subscriberArgs,
                                                         CancellationToken cancellationToken) {
        // creates a new subscription
        var subscription = new Subscription(topic, messageHandler);

        _logger.CreatingSubscription(subscription.ConsumerTag);

        // creates a new channel for this subscription instance.
        // topic is the exchange name.
        var channel = await _channelFactory.CreateChannelAsync(topic, cancellationToken)
                                           .ConfigureAwait(false);

        // creates the consumer for the channel
        var consumer = new AsyncEventingBasicConsumer(channel);

        // set received callback
        consumer.ReceivedAsync += (consumerChannel, deliverEventArgs)
            => OnMessageAsync((IChannel)consumerChannel,
                subscription,
                deliverEventArgs,
                subscriberArgs);

        // startup the consumer
        var startupResult = await channel.BasicConsumeAsync(subscriberArgs.GetQueueName(),
                                              subscriberArgs.GetAutoAck(),
                                              subscription.ConsumerTag,
                                              consumer,
                                              cancellationToken)
                                         .ConfigureAwait(false);

        _logger.SubscriptionCreationComplete(subscription.ConsumerTag, startupResult);

        return new CacheEntry(subscription, channel);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _semaphoreSlim?.Dispose();
        }

        _semaphoreSlim = null;
        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private SemaphoreSlim GetSemaphoreSlim() {
        return _semaphoreSlim ??= new SemaphoreSlim(1, 1);
    }

    private Task OnMessageAsync(IChannel channel,
                                Subscription subscription,
                                BasicDeliverEventArgs deliverEventArgs,
                                SubscriberArgs subscriberArgs) {
        if (TryDeserializeEnvelope(deliverEventArgs, out var envelope) &&
            TryCreateMessageHandler(subscription, out var messageHandler)) {
            return ExecuteMessageHandler(channel,
                messageHandler,
                envelope.Message,
                deliverEventArgs,
                subscriberArgs);
        }

        return NegativeAckAsync(channel, deliverEventArgs, subscriberArgs);
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs deliverEventArgs,
                                        [NotNullWhen(true)] out Envelope? envelope) {
        envelope = null;

        // Our "Envelope" is an array of bytes that contains
        // the serialized (JSON) version of the Envelope instance.
        try { envelope = JsonSerializer.Deserialize<Envelope>(deliverEventArgs.Body.ToArray()); }
        catch (Exception ex) {
            _logger.DeserializeEnvelopeFailed(ex);

            return false;
        }

        if (envelope is null) {
            // Deserialization didn't throw exception, but failed.
            // We were not able to retrieve the envelope for some reason.
            _logger.EnvelopeDeserializationNull(deliverEventArgs);
        }

        return envelope is not null;
    }

    private bool TryCreateMessageHandler(Subscription subscription, [NotNullWhen(true)] out MessageHandlerDelegate? messageHandler) {
        messageHandler = null;

        // Let's try to create the handler delegate
        try { messageHandler = subscription.CreateMessageHandler(); }
        catch (Exception ex) {
            _logger.MessageHandlerCreationFailed(ex);
            return false;
        }

        // For some reason subscription was ok, but we were
        // not able to create the message handler.
        if (messageHandler is null) {
            _logger.MessageHandlerIsNull();
        }

        return messageHandler is not null;
    }

    private static async Task ExecuteMessageHandler(IChannel channel,
                                                    MessageHandlerDelegate messageHandler,
                                                    object message,
                                                    BasicDeliverEventArgs deliverEventArgs,
                                                    SubscriberArgs subscriberArgs) {
        var ack = true;

        try {
            await messageHandler(message, deliverEventArgs.CancellationToken)
               .ConfigureAwait(false);
        }
        catch { ack = false; }

        if (ack) {
            await PositiveAckAsync(channel, deliverEventArgs, subscriberArgs)
               .ConfigureAwait(false);
        }
        else {
            await NegativeAckAsync(channel, deliverEventArgs, subscriberArgs)
               .ConfigureAwait(false);
        }
    }

    private static Task PositiveAckAsync(IChannel channel,
                                         BasicDeliverEventArgs deliverEventArgs,
                                         SubscriberArgs subscriberArgs) {
        if (!subscriberArgs.GetAckOnSuccess() || subscriberArgs.GetAutoAck()) {
            return Task.CompletedTask;
        }

        return channel.BasicAckAsync(deliverEventArgs.DeliveryTag,
                           subscriberArgs.GetAckMultiple(),
                           deliverEventArgs.CancellationToken)
                      .AsTask();
    }

    private static Task NegativeAckAsync(IChannel channel,
                                         BasicDeliverEventArgs deliverEventArgs,
                                         SubscriberArgs subscriberArgs) {
        if (!subscriberArgs.GetNAckOnFailure()) {
            return Task.CompletedTask;
        }

        return channel.BasicNackAsync(deliverEventArgs.DeliveryTag,
                           subscriberArgs.GetNAckMultiple(),
                           subscriberArgs.GetRequeueOnFailure(),
                           deliverEventArgs.CancellationToken)
                      .AsTask();
    }

    internal static string CreateCacheKey(string topic, Delegate messageHandler) {
        var method = messageHandler.Method;
        var targetHashCode = messageHandler.Target?.GetHashCode() ?? 0;
        var parameters = method.GetParameters()
                               .Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}")
                               .ToArray();

        // Example: [new_order_created]: (ref: 123) Namespace.Class.Method(string value, int value, ...)
        var tag = $"[{topic}]: (ref: {targetHashCode}) {method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";

        return tag.GetBytes().ToBase64String();
    }
}