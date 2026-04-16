using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ProducerContextExtensions {
    private const string EXCHANGE_NAME = "ExchangeName";
    private const string ROUTING_KEYS = "RoutingKeys";
    private const string MANDATORY = "Mandatory";
    private const string USE_PREFETCH = "UsePrefetch";

    extension(ProducerContext self) {
        public string ExchangeName {
            get => self[EXCHANGE_NAME] as string ?? string.Empty;
            set => self[EXCHANGE_NAME] = value;
        }

        public bool HasRoutingKeys {
            get => self.RoutingKeys.Length > 0;
        }

        public string[] RoutingKeys {
            get => self[ROUTING_KEYS] as string[] ?? [];
            set => self[ROUTING_KEYS] = value;
        }

        public bool Mandatory {
            get => self[MANDATORY] is true;
            set => self[MANDATORY] = value;
        }

        public bool UsePrefetch {
            get => self[USE_PREFETCH] is true;
            set => self[USE_PREFETCH] = value;
        }

        public BasicProperties CreateBasicProperties() {
            return new BasicProperties().FillWith(self);
        }
    }
}
