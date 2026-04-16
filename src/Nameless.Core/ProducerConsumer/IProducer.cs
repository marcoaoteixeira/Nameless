namespace Nameless.ProducerConsumer;

public interface IProducer {
    Task ProduceAsync(string topic, object message, ProducerContext context, CancellationToken cancellationToken);
}