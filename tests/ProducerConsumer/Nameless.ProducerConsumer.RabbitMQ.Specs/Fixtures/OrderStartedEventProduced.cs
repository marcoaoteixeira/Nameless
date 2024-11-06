namespace Nameless.ProducerConsumer.RabbitMQ.Fixtures;
public record OrderStartedEventProduced {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
}
