using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class ConsumerService : IConsumerService, IDisposable {
    private readonly ConcurrentDictionary<string, IDisposable> _cache = [];
    private readonly IModel _channel;
    private readonly ILogger _logger;

    private bool _disposed;

    public ConsumerService(IModel channel, ILogger<ConsumerService> logger) {
        _channel = Prevent.Argument.Null(channel);
        _logger = Prevent.Argument.Null(logger);
    }

    ~ConsumerService() {
        Dispose(disposing: false);
    }

    /// <summary>
    /// Registers a message handler.
    /// </summary>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <param name="topic">
    ///     The topic to listen. If queue name is set through
    ///     <c>ConsumerArgs.SetQueueName()</c> method, it will
    ///     take precedence over <paramref name="topic"/>.
    /// </param>
    /// <param name="handler">The handler.</param>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    public Registration<T> Register<T>(string topic, MessageHandler<T> handler, ConsumerArgs? args = null) {
        BlockAccessAfterDispose();

        var innerArgs = args ?? ConsumerArgs.Empty;

        // create callback tag
        var tag = GenerateTag(handler);

        var registration = _cache.GetOrAdd(tag, key => {
            _logger.ConsumerRegistration(tag);
            _logger.ConsumerArguments(innerArgs.ToJson());

            // creates registration
            var registration = new Registration<T>(key, topic, handler);

            // creates the consumer event
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += (_, deliverEventArgs)
                => OnMessageAsync(registration, deliverEventArgs, innerArgs);

            // attach the consumer
            var queue = innerArgs.GetQueueName();
            _ = _channel.BasicConsume(queue: string.IsNullOrWhiteSpace(queue)
                                          ? topic
                                          : queue,
                                      autoAck: innerArgs.GetAutoAck(),
                                      consumerTag: key,
                                      consumer: consumer);

            return registration;
        });

        return (Registration<T>)registration;
    }

    public bool Unregister<T>(Registration<T> registration) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(registration);

        if (_cache.Remove(registration.Tag, out var disposable)) {
            _channel.BasicCancel(registration.Tag);
            disposable.Dispose();

            return true;
        }

        return false;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(ConsumerService));
        }
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

    private Task OnMessageAsync<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
        if (!TryDeserializeEnvelope(deliverEventArgs, consumerArgs, out var envelope)) {
            return Task.CompletedTask;
        }

        if (!TryExtractMessage<T>(envelope, deliverEventArgs, consumerArgs, out var message)) {
            return Task.CompletedTask;
        }

        if (!TryCreateHandler(registration, deliverEventArgs, consumerArgs, out var handler)) {
            return Task.CompletedTask;
        }

        return HandleMessageAsync(handler: handler,
                                  message: message,
                                  deliverEventArgs: deliverEventArgs,
                                  consumerArgs: consumerArgs);
    }

    private bool TryDeserializeEnvelope(BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out Envelope? envelope) {
        envelope = null;

        // Our "Envelope" is an array of bytes that contains
        // the serialized (JSON) version of the Envelope instance.
        try { envelope = deliverEventArgs.GetEnvelope(); }
        catch (Exception ex) {
            _logger.EnvelopeDeserializationException(ex);
            return false;
        }

        // Deserialization didn't throw exception, but failed.
        // We were not able to retrieve the envelope for some reason.
        if (envelope is null) {
            _logger.EnvelopeDeserializationFailure();

            // Send NACK (consumer args defined)
            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

            return false;
        }

        return true;
    }

    private bool TryExtractMessage<T>(Envelope envelope, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out T? message) {
        message = default;

        // Let's check if our message type is the same as the 

        // Here, if the envelope.Message is not a JsonElement.
        // We'll log the info and return nothing.
        if (envelope.Message is not JsonElement json) {
            _logger.EnvelopeMessageNotValidJsonElement();

            // Send NACK (consumer args defined)
            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

            return false;
        }

        // If it is a JsonElement, we'll deserialize it to the type
        // that the handler is expecting.
        try { message = json.Deserialize<T>(); } catch (Exception ex) {
            _logger.MessageDeserializationException(ex);
            return false;
        }

        // For some reason, we were not able to deserialize the message
        if (message is null) {
            // Let's log this info
            _logger.MessageDeserializationFailure<T>();

            // Send NACK (consumer args defined)
            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

            return false;
        }

        return true;
    }

    private bool TryCreateHandler<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out MessageHandler<T>? handler) {
        handler = null;

        // Let's try to create the handler delegate
        try { handler = registration.CreateHandler(); }
        catch (Exception ex) {
            // Ok, registration was disposed?
            if (ex is ObjectDisposedException) {
                Unregister(registration);
            }

            // Log the error
            _logger.ConsumerHandlerCreationFailure(ex);

            // Send NACK (consumer args defined)
            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

            return false;
        }

        // For some reason registration was ok, but we were
        // not able to create the delegate.
        if (handler is null) {
            // Log notification
            _logger.MessageHandlerNotFound();

            // Send NACK (consumer args defined)
            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

            return false;
        }

        return true;
    }

    private async Task HandleMessageAsync<T>(MessageHandler<T> handler, T message, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
        try {
            // Let's execute the handler with the received message
            await handler(message);

            // If everything goes ok, let us ack the message received.
            // Check consumer args for 
            PositiveAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);

        } catch (Exception ex) {
            _logger.MessageHandlerException(ex);

            NegativeAck(channel: _channel,
                        deliverEventArgs: deliverEventArgs,
                        consumerArgs: consumerArgs);
        }
    }

    private static string GenerateTag<T>(MessageHandler<T> handler) {
        var method = handler.Method;
        var parameters = method.GetParameters()
                               .Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}")
                               .ToArray();

        var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
        var buffer = signature.GetBytes();

        return buffer.ToBase64String();
    }

    private static void PositiveAck(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
        if (!consumerArgs.GetAckOnSuccess() || consumerArgs.GetAutoAck()) {
            return;
        }

        channel.BasicAck(deliveryTag: deliverEventArgs.DeliveryTag,
                         multiple: consumerArgs.GetAckMultiple());
    }

    private static void NegativeAck(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
        if (!consumerArgs.GetNAckOnFailure()) {
            return;
        }

        channel.BasicNack(deliveryTag: deliverEventArgs.DeliveryTag,
                          multiple: consumerArgs.GetNAckMultiple(),
                          requeue: consumerArgs.GetRequeueOnFailure());
    }
}