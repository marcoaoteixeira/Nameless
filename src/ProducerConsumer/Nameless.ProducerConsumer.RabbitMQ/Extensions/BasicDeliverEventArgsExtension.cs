using System.Text.Json;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;

internal static class BasicDeliverEventArgsExtension {
    internal static Envelope? GetEnvelope(this BasicDeliverEventArgs self)
        => JsonSerializer.Deserialize<Envelope>(self.Body.ToArray());
}