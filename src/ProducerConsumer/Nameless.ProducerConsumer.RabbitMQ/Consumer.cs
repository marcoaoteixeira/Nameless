using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class Consumer : IConsumer, IDisposable {
    private readonly Dictionary<string, CacheEntry> _cache = [];

    private readonly IChannelProvider _channelProvider;
    private readonly ILogger<Consumer> _logger;

    private SemaphoreSlim? _semaphoreSlim;
    private bool _disposed;

    public Consumer(IChannelProvider channelProvider, ILogger<Consumer> logger) {
        _channelProvider = Prevent.Argument.Null(channelProvider);
        _logger = Prevent.Argument.Null(logger);
    }

    ~Consumer() {
        Dispose(disposing: false);
    }

    public async Task<Registration<TMessage>> RegisterAsync<TMessage>(string topic,
                                                                      MessageHandlerAsync<TMessage> messageHandler,
                                                                      ConsumerArgs args,
                                                                      CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        await GetSemaphoreSlim().WaitAsync(cancellationToken);

        try {
            var consumerTag = GenerateConsumerTag(messageHandler);

            if (_cache.TryGetValue(consumerTag, out var cacheEntry)) {
                return (Registration<TMessage>)cacheEntry.Registration;
            }

            _cache[consumerTag] = await CreateNewCacheEntry(consumerTag,
                                                            topic,
                                                            messageHandler,
                                                            args,
                                                            cancellationToken);

            return (Registration<TMessage>)_cache[consumerTag].Registration;
        }
        catch (Exception ex) { _logger.ConsumerRegistrationFailed(ex); throw; }
        finally { GetSemaphoreSlim().Release(); }
    }

    public async Task<bool> UnregisterAsync<T>(Registration<T> registration, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(registration);

        await GetSemaphoreSlim().WaitAsync(cancellationToken);

        try { await RemoveRegistrationFromCacheAsync(registration.Tag,
                                                     cancellationToken); }
        finally { GetSemaphoreSlim().Release(); }

        return true;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var registrations = _cache.Values.ToArray();

            _cache.Clear();

            foreach (var registration in registrations) {
                registration.Dispose();
            }
        }

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

    private async Task<CacheEntry> CreateNewCacheEntry<TMessage>(string consumerTag,
                                                                             string topic,
                                                                             MessageHandlerAsync<TMessage> handler,
                                                                             ConsumerArgs args,
                                                                             CancellationToken cancellationToken) {
        _logger.RegisteringNewConsumer(consumerTag, args.ToJson());

        // creates a new registration instance
        var registration = new Registration<TMessage>(consumerTag, topic, handler);

        // creates a new channel for this registration instance.
        // topic is the exchange name.
        var channel = await _channelProvider.CreateChannelAsync(topic,
                                                                cancellationToken)
                                            .ConfigureAwait(continueOnCapturedContext: false);

        // creates the consumer for the channel
        var consumer = new AsyncEventingBasicConsumer(channel);

        // set received callback
        consumer.ReceivedAsync += (consumerChannel, deliverEventArgs)
            => OnMessageAsync((IChannel)consumerChannel,
                              registration,
                              deliverEventArgs,
                              args);

        // startup the consumer
        var queue = args.GetQueueName();
        var startupResult = await channel.BasicConsumeAsync(queue: string.IsNullOrWhiteSpace(queue)
                                                                ? topic
                                                                : queue,
                                                            args.GetAutoAck(),
                                                            consumerTag,
                                                            consumer,
                                                            cancellationToken)
                                         .ConfigureAwait(continueOnCapturedContext: false);

        _logger.ConsumerStartupResult(startupResult);

        return new CacheEntry(registration, channel);
    }

    private async Task OnMessageAsync<T>(IChannel channel,
                                         Registration<T> registration,
                                         BasicDeliverEventArgs deliverEventArgs,
                                         ConsumerArgs consumerArgs) {
        if (TryDeserializeEnvelope(deliverEventArgs, out var envelope) &&
            TryExtractMessage<T>(envelope, out var message) &&
            TryCreateHandler(registration, out var handler)) {

            await HandleMessageAsync(channel,
                                     handler,
                                     message,
                                     deliverEventArgs,
                                     consumerArgs);
        }

        await NegativeAckAsync(channel: channel,
                               deliverEventArgs: deliverEventArgs,
                               consumerArgs: consumerArgs);
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs deliverEventArgs,
                                        [NotNullWhen(returnValue: true)] out Envelope? envelope) {
        envelope = null;

        // Our "Envelope" is an array of bytes that contains
        // the serialized (JSON) version of the Envelope instance.
        try { envelope = deliverEventArgs.GetEnvelope(); } catch (Exception ex) {
            _logger.ErrorOnEnvelopeDeserialization(ex);

            return false;
        }

        if (envelope is null) {
            // Deserialization didn't throw exception, but failed.
            // We were not able to retrieve the envelope for some reason.
            _logger.NullEnvelopeDeserialized();
        }

        return envelope is not null;
    }

    private bool TryExtractMessage<T>(Envelope envelope,
                                      [NotNullWhen(returnValue: true)] out T? message) {
        message = default;

        // Let's check if our message type is the same as the 

        // Here, if the envelope.Message is not a JsonElement.
        // We'll log the info and return nothing.
        if (envelope.Message is not JsonElement json) {
            _logger.EnvelopeMessageNotValidJsonElement();

            return false;
        }

        // If it is a JsonElement, we'll deserialize it to the type
        // that the handler is expecting.
        try { message = json.Deserialize<T>(); } catch (Exception ex) {
            _logger.ErrorOnMessageDeserialization(ex);

            return false;
        }

        // For some reason, we were not able to deserialize the message
        if (message is null) {
            // Let's log this info
            _logger.NullMessageDeserialization<T>();
        }

        return message is not null;
    }

    private bool TryCreateHandler<T>(Registration<T> registration,
                                     [NotNullWhen(returnValue: true)] out MessageHandlerAsync<T>? handler) {
        handler = null;

        // Let's try to create the handler delegate
        try { handler = registration.CreateMessageHandler(); } catch (Exception ex) {
            HandleObjectDisposedException(registration, ex);

            // Log the error
            _logger.ErrorOnConsumerHandlerCreation(ex);

            return false;
        }

        // For some reason registration was ok, but we were
        // not able to create the delegate.
        if (handler is null) {
            // Log notification
            _logger.MessageHandlerNotFound();
        }

        return handler is not null;
    }

    private void HandleObjectDisposedException<T>(Registration<T> registration, Exception exception) {
        if (exception is not ObjectDisposedException) { return; }

        // Ok, registration was disposed outside
        try {
            RemoveRegistrationFromCacheAsync(registration.Tag, CancellationToken.None)
                .ConfigureAwait(continueOnCapturedContext: false)
                .GetAwaiter()
                .GetResult();
        } catch (Exception ex) { _logger.RemoveRegistrationFromCacheFailed(ex); }
    }

    private async Task HandleMessageAsync<TMessage>(IChannel channel,
                                                    MessageHandlerAsync<TMessage> handler,
                                                    TMessage message,
                                                    BasicDeliverEventArgs deliverEventArgs,
                                                    ConsumerArgs consumerArgs) {
        try {
            // Let's execute the handler with the received message
            await handler(message, deliverEventArgs.CancellationToken);

            // If everything goes ok, let us ack the message received.
            // Check consumer args for 
            await PositiveAckAsync(channel: channel,
                                   deliverEventArgs: deliverEventArgs,
                                   consumerArgs: consumerArgs);

        } catch (Exception ex) {
            _logger.ErrorOnHandleMessage(ex);

            await NegativeAckAsync(channel: channel,
                                   deliverEventArgs: deliverEventArgs,
                                   consumerArgs: consumerArgs);
        }
    }

    private async Task RemoveRegistrationFromCacheAsync(string consumerTag, CancellationToken cancellationToken) {
        if (_cache.Remove(consumerTag, out var cacheEntry)) {
            await cacheEntry.Channel
                            .BasicCancelAsync(consumerTag,
                                              noWait: true,
                                              cancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

            cacheEntry.Registration.Dispose();
            await cacheEntry.Channel
                            .DisposeAsync()
                            .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private static string GenerateConsumerTag<T>(MessageHandlerAsync<T> handler) {
        var method = handler.Method;
        var parameters = method.GetParameters()
                               .Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}")
                               .ToArray();

        var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
        var buffer = signature.GetBytes();

        return buffer.ToBase64String();
    }

    private static Task PositiveAckAsync(IChannel channel,
                                         BasicDeliverEventArgs deliverEventArgs,
                                         ConsumerArgs consumerArgs) {
        if (!consumerArgs.GetAckOnSuccess() || consumerArgs.GetAutoAck()) {
            return Task.CompletedTask;
        }

        return channel.BasicAckAsync(deliverEventArgs.DeliveryTag,
                                     consumerArgs.GetAckMultiple(),
                                     deliverEventArgs.CancellationToken)
                      .AsTask();
    }

    private static Task NegativeAckAsync(IChannel channel,
                                         BasicDeliverEventArgs deliverEventArgs,
                                         ConsumerArgs consumerArgs) {
        if (!consumerArgs.GetNAckOnFailure()) {
            return Task.CompletedTask;
        }

        return channel.BasicNackAsync(deliverEventArgs.DeliveryTag,
                                      consumerArgs.GetNAckMultiple(),
                                      consumerArgs.GetRequeueOnFailure(),
                                      deliverEventArgs.CancellationToken)
                      .AsTask();
    }
}