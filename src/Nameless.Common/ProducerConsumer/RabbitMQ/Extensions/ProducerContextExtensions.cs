using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Extension properties and methods for <see cref="ProducerContext"/> that expose
///     RabbitMQ producer-specific settings stored in the context dictionary.
/// </summary>
public static class ProducerContextExtensions {
    private const string EXCHANGE_NAME = "ExchangeName";
    private const string ROUTING_KEYS = "RoutingKeys";
    private const string MANDATORY = "Mandatory";
    private const string USE_PREFETCH = "UsePrefetch";

    /// <param name="self">The current <see cref="ProducerContext"/> instance.</param>
    extension(ProducerContext self) {
        /// <summary>
        ///     Gets or sets the name of the RabbitMQ exchange to publish messages to.
        /// </summary>
        public string ExchangeName {
            get => self[EXCHANGE_NAME] as string ?? string.Empty;
            set => self[EXCHANGE_NAME] = value;
        }

        /// <summary>
        ///     Gets a value indicating whether any routing keys have been configured.
        /// </summary>
        public bool HasRoutingKeys {
            get => self.RoutingKeys.Length > 0;
        }

        /// <summary>
        ///     Gets or sets the routing keys used when publishing to the exchange.
        /// </summary>
        public string[] RoutingKeys {
            get => self[ROUTING_KEYS] as string[] ?? [];
            set => self[ROUTING_KEYS] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the message publish is mandatory
        ///     (broker must route it to at least one queue).
        /// </summary>
        public bool Mandatory {
            get => self[MANDATORY] is true;
            set => self[MANDATORY] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether channel prefetch should be applied.
        /// </summary>
        public bool UsePrefetch {
            get => self[USE_PREFETCH] is true;
            set => self[USE_PREFETCH] = value;
        }

        /// <summary>
        ///     Creates a <see cref="BasicProperties"/> instance populated from this producer context.
        /// </summary>
        /// <returns>A <see cref="BasicProperties"/> filled with the context's message properties.</returns>
        public BasicProperties CreateBasicProperties() {
            return new BasicProperties().FillWith(self);
        }
    }
}
