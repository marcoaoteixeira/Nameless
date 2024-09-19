namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record QueueOptions {
    public static QueueOptions Default => new();

    private Dictionary<string, object>? _arguments;
    private BindingOptions[]? _bindings;
    private string? _name;

    public string Name {
        get => _name.WithFallback(Root.Defaults.QUEUE_NAME);
        set => _name = value;
    }

    public bool Durable { get; set; } = true;

    public bool Exclusive { get; set; }

    public bool AutoDelete { get; set; }

    public Dictionary<string, object> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    public BindingOptions[] Bindings {
        get => _bindings ??= [BindingOptions.Default];
        set => _bindings = value.IsNullOrEmpty()
            ? [BindingOptions.Default]
            : value;
    }
}