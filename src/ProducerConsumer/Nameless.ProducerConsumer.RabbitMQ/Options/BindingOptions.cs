namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record BindingOptions {
    public static BindingOptions Default => new();

    private string? _routingKey;
    private Dictionary<string, object>? _arguments;

    public string RoutingKey {
        get => _routingKey ??= string.Empty;
        set => _routingKey = value;
    }

    public Dictionary<string, object> Arguments {
        get => _arguments ??= [];
        set => _arguments = value;
    }
}