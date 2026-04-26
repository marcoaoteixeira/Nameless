namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

public record Header {
    public string? MessageID { get; init; }
    public string? CorrelationID { get; init; }
    public long Timestamp { get; init; }
}