namespace Nameless.ProducerConsumer;

public static class ProducerExtensions {
    extension(IProducer self) {
        public Task ProduceAsync(string topic, object message, CancellationToken cancellationToken) {
            return self.ProduceAsync(topic, message, context: [], cancellationToken);
        }
    }
}