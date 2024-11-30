namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record BindingSettings {
    private string? _routingKey;
    public string RoutingKey {
        get => _routingKey ??= string.Empty;
        set => _routingKey = value;
    }

    private Dictionary<string, object?>? _arguments;
    public Dictionary<string, object?> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }
}