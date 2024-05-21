using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class ConsumerService : IConsumerService, IDisposable {
        #region Private Read-Only Fields

        private readonly ConcurrentDictionary<string, IDisposable> _cache = [];
        private readonly IModel _channel;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Constructors

        public ConsumerService(IModel channel)
            : this(channel, NullLogger<ConsumerService>.Instance) { }

        public ConsumerService(IModel channel, ILogger<ConsumerService> logger) {
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
            if (!TryCreateHandler(registration, deliverEventArgs, consumerArgs, out var handler)) {
                return Task.CompletedTask;
            }

            if (!TryDeserializeEnvelope<T>(deliverEventArgs, consumerArgs, out var envelope)) {
                return Task.CompletedTask;
            }

            if (!TryExtractMessage<T>(envelope, deliverEventArgs, consumerArgs, out var message)) {
                return Task.CompletedTask;
            }

            return HandleMessageAsync(
                handler: handler,
                message: message,
                deliverEventArgs: deliverEventArgs,
                consumerArgs: consumerArgs
            );
        }

        private bool TryCreateHandler<T>(Registration<T> registration, BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out MessageHandler<T>? handler) {
            handler = null;

            // Let's try create the handler delegate
            try { handler = registration.CreateHandler(); }
            catch (Exception ex) {
                // Ok, registration was disposed?
                if (ex is ObjectDisposedException) {
                    Unregister(registration);
                }

                // Log the error
                _logger.LogError(exception: ex,
                                 message: "Consumer handler creation failed. Reason: {Error}",
                                 args: [ex.Message]);

                // Send NACK (consumer args defined)
                NegativeAck(channel: _channel,
                            deliverEventArgs: deliverEventArgs,
                            consumerArgs: consumerArgs);

                return false;
            }

            // For some reason, registration was ok but we were
            // not able to create the delegate.
            if (handler is null) {
                // Log notification
                _logger.LogWarning(message: "No suitable handler found.");

                // Send NACK (consumer args defined)
                NegativeAck(channel: _channel,
                            deliverEventArgs: deliverEventArgs,
                            consumerArgs: consumerArgs);

                return false;
            }

            return true;
        }

        private bool TryDeserializeEnvelope<T>(BasicDeliverEventArgs deliverEventArgs, ConsumerArgs consumerArgs, [NotNullWhen(returnValue: true)] out Envelope? envelope) {
            envelope = deliverEventArgs.GetEnvelope();

            // We were not able to retrieve the envelope for some reason.
            if (envelope is null) {
                _logger.LogWarning(message: "Envelope deserialization failed.");

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

            // Here, if the envelope.Message is not a JsonElement.
            // We'll log the info and return nothing.
            if (envelope.Message is not JsonElement json) {
                _logger.LogError("Message was not valid JSON.");

                // Send NACK (consumer args defined)
                NegativeAck(channel: _channel,
                            deliverEventArgs: deliverEventArgs,
                            consumerArgs: consumerArgs);

                return false;
            }

            // If it is a JsonElement, we'll deserialize it to the type
            // that the handler is expecting.
            message = json.Deserialize<T>();

            // For some reason, we were not able to deserialize the message
            if (message is null) {
                // Let's log this info
                _logger.LogWarning(message: "Unable to deserialize the message to expecting type {MessageType}.",
                                   args: [typeof(T)]);

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

                // If everything goes ok, let's ack the message received.
                // Check consumer args for 
                PositiveAck(channel: _channel,
                            deliverEventArgs: deliverEventArgs,
                            consumerArgs: consumerArgs);

            } catch (Exception ex) {
                _logger.LogError(exception: ex,
                                 message: "Error when handling the message. Reason: {Message}",
                                 args: [ex.Message]);

                NegativeAck(channel: _channel,
                            deliverEventArgs: deliverEventArgs,
                            consumerArgs: consumerArgs);
            }
        }

        #endregion

        #region Private Static Methods

        private static string GenerateTag<T>(MessageHandler<T> handler) {
            var method = handler.Method;
            var parameters = method
                .GetParameters()
                .Select(_ => $"{_.ParameterType.Name} {_.Name}")
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

        #endregion

        #region IConsumerService Members

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
                _logger.LogInformation("Initialize registration of consumer: {tag}", key);
                _logger.LogInformation("Consumer arguments: {json}", innerArgs.ToJson());

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

            Guard.Against.Null(registration, nameof(registration));

            if (_cache.Remove(registration.Tag, out var disposable)) {
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
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
