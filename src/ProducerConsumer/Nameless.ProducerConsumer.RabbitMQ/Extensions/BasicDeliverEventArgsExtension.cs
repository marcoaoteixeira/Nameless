using System.Text.Json;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ {
    internal static class BasicDeliverEventArgsExtension {
        #region Internal Static Methods

        internal static Envelope? GetEnvelope(this BasicDeliverEventArgs self)
            => JsonSerializer
                .Deserialize<Envelope>(
                    self.Body.ToArray()
                );

        #endregion
    }
}
