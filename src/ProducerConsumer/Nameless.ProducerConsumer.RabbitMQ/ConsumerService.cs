﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ConsumerService : IConsumerService, IDisposable {
        #region Private Read-Only Fields

        private readonly IModel _channel;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private ConcurrentDictionary<string, IDisposable> _registrations = new();
        private bool _disposed;

        #endregion

        #region Public Constructors

        public ConsumerService(IModel channel)
            : this(channel, NullLogger.Instance) { }

        public ConsumerService(IModel channel, ILogger logger) {
            _channel = Guard.Against.Null(channel, nameof(channel));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Destructor

        ~ConsumerService() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(ConsumerService));

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                var registrations = _registrations.Values.ToArray();
                _registrations.Clear();

                foreach (var registration in registrations) {
                    registration.Dispose();
                }
            }

            _registrations = null!;
            _disposed = true;
        }

        private async Task OnMessage<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            if (!TryCreateHandler(registration, deliverEventArgs, consumerArgs, out var handler)) {
                return;
            }

            if (!TryDeserializeEnvelope(registration, deliverEventArgs, consumerArgs, out var envelope)) {
                return;
            }

            if (!TryExtractMessage(envelope, registration, deliverEventArgs, consumerArgs, out var message)) {
                return;
            }

            await HandleMessageAsync(
                handler,
                message,
                registration,
                deliverEventArgs,
                consumerArgs
            );
        }

        private bool TryCreateHandler<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)]out MessageHandler<T>? handler) {
            handler = null;

            try { handler = registration.CreateHandler(); }
            catch (Exception ex) {
                if (ex is ObjectDisposedException) {
                    Unregister(registration);
                }

                _logger.LogError(ex, "{registration}: Handler creation failed. Reason: {Message}", registration, ex.Message);
                NAck(_channel, deliverEventArgs, consumerArgs);

                return false;
            }

            if (handler is null) {
                _logger.LogWarning("{registration}: No suitable handler found.", registration);
                NAck(_channel, deliverEventArgs, consumerArgs);

                return false;
            }

            return true;
        }

        private bool TryDeserializeEnvelope<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out Envelope? envelope) {
            envelope = JsonSerializer.Deserialize<Envelope>(deliverEventArgs.Body.ToArray());
            if (envelope is null) {
                _logger.LogWarning("{registration}: Envelope deserialization failed.", registration);
                NAck(_channel, deliverEventArgs, consumerArgs);

                return false;
            }

            return true;
        }

        private bool TryExtractMessage<T>(Envelope envelope, Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out T? message) {
            message = default;

            // Here, envelope.Message is a JsonElement.
            // So, we'll deserialize it to the type that the handler is expecting.
            if (envelope.Message is not JsonElement json) {
                _logger.LogWarning(
                    message: "{registration}: Message is not a {type}.",
                    args: [registration, typeof(JsonElement)]
                );
                NAck(_channel, deliverEventArgs, consumerArgs);

                return false;
            }

            message = json.Deserialize<T>();

            if (message is null) {
                _logger.LogWarning(
                    message: "{registration}: Unable to deserialize the message to expecting type {type}.",
                    args: [registration, typeof(T)]
                );
                NAck(_channel, deliverEventArgs, consumerArgs);

                return false;
            }

            return true;
        }

        private async Task HandleMessageAsync<T>(MessageHandler<T> handler, T message, Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            try {
                await handler(message);
                Ack(_channel, deliverEventArgs, consumerArgs);
            } catch (Exception ex) {
                _logger.LogError(
                    exception: ex,
                    message: "{registration}: Error when handling the message. Reason: {Message}",
                    args: [registration, ex.Message]
                );
                NAck(_channel, deliverEventArgs, consumerArgs);
            }
        }

        #endregion

        #region Private Static Methods

        private static string GenerateTag<T>(MessageHandler<T> handler) {
            if (handler is null) { return string.Empty; }

            var method = handler.Method;
            var parameters = method.GetParameters().Select(_ => $"{_.ParameterType.Name} {_.Name}").ToArray();
            var signature = $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", parameters)})";
            var buffer = Encoding.UTF8.GetBytes(signature);

            return Convert.ToBase64String(buffer);
        }

        private static void Ack(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            if (!consumerArgs.GetAckOnSuccess()) { return; }

            channel.BasicAck(
                deliveryTag: deliverEventArgs.DeliveryTag,
                multiple: consumerArgs.GetAckMultiple()
            );
        }

        private static void NAck(IModel channel, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs) {
            if (!consumerArgs.GetNAckOnFailure()) { return; }

            channel.BasicNack(
                deliveryTag: deliverEventArgs.DeliveryTag,
                multiple: consumerArgs.GetNAckMultiple(),
                requeue: consumerArgs.GetRequeueOnFailure()
            );
        }

        #endregion

        #region IConsumerService Members

        public Registration<T> Register<T>(string topic, MessageHandler<T> handler, ConsumerArgs? args = null) {
            BlockAccessAfterDispose();

            var consumerArgs = args ?? ConsumerArgs.Empty;

            // create callback tag
            var tag = GenerateTag(handler);

            var registration = _registrations.GetOrAdd(tag, tag => {
                _logger.LogInformation("Initialize registration of consumer: {tag}", tag);
                _logger.LogInformation("Consumer arguments: {json}", consumerArgs.ToJson());

                // creates registration
                var registration = new Registration<T>(tag, topic, handler);

                // creates the consumer event
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += (sender, deliverEventArgs)
                    => OnMessage(registration, deliverEventArgs, consumerArgs);

                // attach the consumer
                var consumerTag = _channel.BasicConsume(
                    queue: topic,
                    autoAck: consumerArgs.GetAutoAck(),
                    consumerTag: tag,
                    consumer: consumer
                );

                return registration;
            });

            return (Registration<T>)registration;
        }

        public bool Unregister<T>(Registration<T> registration) {
            BlockAccessAfterDispose();

            Guard.Against.Null(registration, nameof(registration));

            if (_registrations.Remove(registration.Tag, out var disposable)) {
                _channel.BasicCancel(registration.Tag);
                disposable.Dispose();

                return true;
            }

            return false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}
