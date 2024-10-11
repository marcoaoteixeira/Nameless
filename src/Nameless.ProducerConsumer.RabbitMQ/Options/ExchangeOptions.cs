namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record ExchangeOptions {
    public static ExchangeOptions Default => new();

    private Dictionary<string, object>? _arguments;
    private QueueOptions[]? _queues;
    private string? _name;

    public string Name {
        get => _name ??= Internals.Defaults.EXCHANGE_NAME;
        set => _name = value;
    }

    public ExchangeType Type { get; set; }

    public bool Durable { get; set; } = true;

    public bool AutoDelete { get; set; }

    public Dictionary<string, object> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    public QueueOptions[] Queues {
        get => _queues ??= [];
        set => _queues = value;
    }
}