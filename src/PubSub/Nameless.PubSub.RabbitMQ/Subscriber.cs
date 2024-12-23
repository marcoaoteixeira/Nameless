using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.PubSub.RabbitMQ.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.PubSub.RabbitMQ;

public sealed class Subscriber : ISubscriber, IDisposable {
    private readonly IChannelFactory _channelFactory;
    private readonly ILogger<Subscriber> _logger;

    private SemaphoreSlim? _semaphoreSlim;
    private bool _disposed;

    private Dictionary<string, CacheEntry> Cache { get; } = [];

    public Subscriber(IChannelFactory channelFactory, ILogger<Subscriber> logger) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _logger = Prevent.Argument.Null(logger);
    }

    ~Subscriber() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The parameter <paramref name="topic"/> is the RabbitMQ exchange name.
    /// The queue name should be provided through the method <see cref="SubscriberArgsExtension.SetQueueName"/>
    /// applied to the parameter <paramref name="args"/> before calling <see cref="SubscribeAsync"/>.
    /// </remarks>
    public async Task<string> SubscribeAsync(string topic,
                                             Func<object, CancellationToken, Task> messageHandler,
                                             SubscriberArgs args,
                                             CancellationToken cancellationToken) {
        Prevent.Argument.Null(topic);
        Prevent.Argument.Null(messageHandler);

        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);

        try {
            var cacheKey = CreateCacheKey(topic, messageHandler);

            if (Cache.TryGetValue(cacheKey, out _)) {
                return cacheKey;
            }

            Cache[cacheKey] = await CreateCacheEntryAsync(topic,
                                                          messageHandler,
                                                          args,
                                                          cancellationToken);
            return cacheKey;
        } finally { GetSemaphoreSlim().Release(); }
    }

    private async Task<CacheEntry> CreateCacheEntryAsync(string topic,
                                                         Func<object, CancellationToken, Task> messageHandler,
                                                         SubscriberArgs subscriberArgs,
                                                         CancellationToken cancellationToken) {
        // creates a new subscription
        var subscription = new Subscription(topic, messageHandler);

        _logger.StartingSubscription(subscription.ID);

        // creates a new channel for this registration instance.
        // topic is the exchange name.
        var channel = await _channelFactory.CreateChannelAsync(topic, cancellationToken)
                                           .ConfigureAwait(continueOnCapturedContext: false);

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
                                                            consumerTag: subscription.ID,
                                                            consumer,
                                                            cancellationToken)
                                         .ConfigureAwait(continueOnCapturedContext: false);

        _logger.SubscriptionStarted(subscription.ID, startupResult);

        return new CacheEntry(subscription, channel);
    }

    public async Task<bool> UnsubscribeAsync(string subscription,
                                        CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(subscription);

        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);

        try {
            if (Cache.Remove(subscription, out var cacheEntry)) {
                await cacheEntry.DisposeAsync()
                                .ConfigureAwait(continueOnCapturedContext: false);
            }
            return true;
        }
        catch { return false; }
        finally { GetSemaphoreSlim().Release(); }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(GetType().Name);
        }
#endif
    }

    private SemaphoreSlim GetSemaphoreSlim()
        => _semaphoreSlim ??= new SemaphoreSlim(initialCount: 1, maxCount: 1);

    private Task OnMessageAsync(IChannel channel,
                                      Subscription subscription,
                                      BasicDeliverEventArgs deliverEventArgs,
                                      SubscriberArgs subscriberArgs) {
        if (TryDeserializeEnvelope(deliverEventArgs, out var envelope) &&
            TryCreateMessageHandler(subscription, out var messageHandler)) {

            return HandleMessageAsync(channel,
                                      messageHandler,
                                      envelope.Message,
                                      deliverEventArgs,
                                      subscriberArgs);
        }

        return NegativeAckAsync(channel, deliverEventArgs, subscriberArgs);
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs deliverEventArgs,
                                        [NotNullWhen(returnValue: true)] out Envelope? envelope) {
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

    private bool TryCreateMessageHandler(Subscription subscription,
                                         [NotNullWhen(returnValue: true)] out MessageHandlerAsync? messageHandler) {
        messageHandler = null;

        // Let's try to create the handler delegate
        try { messageHandler = subscription.CreateMessageHandler(); }
        catch (Exception ex) { _logger.MessageHandlerCreationFailed(ex); return false; }

        // For some reason subscription was ok, but we were
        // not able to create the message handler.
        if (messageHandler is null) {
            _logger.MessageHandlerIsNull();
        }

        return messageHandler is not null;
    }

    private static async Task HandleMessageAsync(IChannel channel,
                                                 MessageHandlerAsync messageHandler,
                                                 object message,
                                                 BasicDeliverEventArgs deliverEventArgs,
                                                 SubscriberArgs subscriberArgs) {
        var ack = true;

        try {
            await messageHandler(message, deliverEventArgs.CancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        catch { ack = false; }

        if (ack) {
            await PositiveAckAsync(channel, deliverEventArgs, subscriberArgs)
                .ConfigureAwait(continueOnCapturedContext: false);
        } else {
            await NegativeAckAsync(channel, deliverEventArgs, subscriberArgs)
                .ConfigureAwait(continueOnCapturedContext: false);
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

        // [new_order_created]: (ref: 123) Namespace.Class.Method(string value, int value, ...)
        var tag = $"[{topic}]: (ref: {targetHashCode}) {method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";

        var buffer = tag.GetBytes();

        return buffer.ToBase64String();
    }

    internal sealed record CacheEntry : IDisposable, IAsyncDisposable {
        private readonly string _consumerTag;

        private Subscription? _subscription;
        private IChannel? _channel;

        private bool _disposed;

        public CacheEntry(Subscription subscription, IChannel channel) {
            _subscription = Prevent.Argument.Null(subscription);
            _channel = Prevent.Argument.Null(channel);

            _consumerTag = _subscription.ID;
        }

        ~CacheEntry() {
            Dispose(disposing: false);
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

        private void Dispose(bool disposing) {
            if (_disposed) { return; }

            if (disposing) {
                _subscription?.Dispose();
                _channel?.Dispose();
            }

            _subscription = null;
            _channel = null;

            _disposed = true;
        }

        private async Task DisposeAsyncCore() {
            if (_channel is not null) {
                try {
                    await _channel.BasicCancelAsync(_consumerTag, noWait: true)
                                  .ConfigureAwait(continueOnCapturedContext: false);
                    await _channel.CloseAsync(Constants.ReplySuccess, $"Closing RabbitMQ channel for subscription '{_consumerTag}'.")
                                  .ConfigureAwait(continueOnCapturedContext: false);
                } catch { /* swallow */ }

                await _channel.DisposeAsync()
                              .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}