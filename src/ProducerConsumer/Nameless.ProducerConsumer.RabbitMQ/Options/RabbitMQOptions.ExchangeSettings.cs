namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record ExchangeSettings {
    private string? _name;
    public string Name {
        get => _name ??= Internals.Defaults.EXCHANGE_NAME;
        set => _name = value;
    }

    public ExchangeType Type { get; set; }

    public bool Durable { get; set; } = true;

    public bool AutoDelete { get; set; }

    private Dictionary<string, object?>? _arguments;
    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    private QueueSettings[]? _queues;
    public QueueSettings[] Queues {
        get => _queues ??= [];
        set => _queues = value;
    }
}