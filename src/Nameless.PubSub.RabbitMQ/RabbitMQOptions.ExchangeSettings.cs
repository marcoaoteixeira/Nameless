namespace Nameless.PubSub.RabbitMQ;

public sealed record ExchangeSettings {
    private Dictionary<string, object?>? _arguments;
    private string? _name;

    private QueueSettings[]? _queues;

    public string Name {
        get => _name ??= Internals.Defaults.ExchangeName;
        set => _name = value;
    }

    public ExchangeType Type { get; set; }

    public bool Durable { get; set; } = true;

    public bool AutoDelete { get; set; }

    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    public QueueSettings[] Queues {
        get => _queues ??= [];
        set => _queues = value;
    }
}