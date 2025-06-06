namespace Nameless.PubSub.RabbitMQ;

public sealed record QueueSettings {
    private static readonly BindingSettings[] DefaultBindings = [new()];

    private Dictionary<string, object?>? _arguments;

    private BindingSettings[] _bindings = DefaultBindings;

    private string? _name;

    public string Name {
        get => _name.WithFallback(Internals.Defaults.QueueName);
        set => _name = value;
    }

    public bool Durable { get; set; }

    public bool Exclusive { get; set; }

    public bool AutoDelete { get; set; }

    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }

    public BindingSettings[] Bindings {
        get => _bindings;
        set => _bindings = value.IsNullOrEmpty() ? DefaultBindings : value;
    }
}