namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public interface IMessageSerializer {
    Task<byte[]> SerializeAsync(object message, Context context, CancellationToken cancellationToken);

    Task<TMessage> DeserializeAsync<TMessage>(byte[] buffer, Context context, CancellationToken cancellationToken);
}