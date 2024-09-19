namespace Nameless.ProducerConsumer.RabbitMQ.Specs.Fixtures;
public record OrderStartedEventReceived {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
}
