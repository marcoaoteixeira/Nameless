namespace Nameless.PubSub.RabbitMQ.Options;

public sealed record QueueSettings {
    private static readonly BindingSettings[] DefaultBindings = [new()];

    private string? _name;
    public string Name {
        get => _name.WithFallback(Defaults.QUEUE_NAME);
        set => _name = value;
    }

    public bool Durable { get; set; }

    public bool Exclusive { get; set; }

    public bool AutoDelete { get; set; }

    private Dictionary<string, object?>? _arguments;
    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    private BindingSettings[] _bindings = DefaultBindings;
    public BindingSettings[] Bindings {
        get => _bindings;
        set => _bindings = value.IsNullOrEmpty() ? DefaultBindings : value;
    }
}