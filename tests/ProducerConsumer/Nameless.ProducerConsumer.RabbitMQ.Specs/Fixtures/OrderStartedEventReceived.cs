namespace Nameless.ProducerConsumer.RabbitMQ.Fixtures;
public record OrderStartedEventReceived {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
}
