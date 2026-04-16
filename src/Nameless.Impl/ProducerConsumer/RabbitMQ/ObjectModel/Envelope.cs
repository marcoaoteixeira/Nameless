namespace Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

public record Envelope {
    public required Header Header { get; init; }

    public required object Message { get; init; }
}