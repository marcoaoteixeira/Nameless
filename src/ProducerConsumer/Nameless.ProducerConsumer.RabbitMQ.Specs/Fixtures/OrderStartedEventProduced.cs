namespace Nameless.ProducerConsumer.RabbitMQ.Specs.Fixtures;
public record OrderStartedEventProduced {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
}
