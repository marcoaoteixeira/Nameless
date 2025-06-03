namespace Nameless.PubSub.RabbitMQ;

public sealed record BindingSettings {
    private Dictionary<string, object?>? _arguments;
    private string? _routingKey;

    public string RoutingKey {
        get => _routingKey ??= string.Empty;
        set => _routingKey = value;
    }

    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }
}