namespace Nameless.ProducerConsumer.RabbitMQ.Options;

public sealed record RabbitMQOptions {
    public static RabbitMQOptions Default => new();

    private ServerOptions? _server;
    private ExchangeOptions[]? _exchanges;

    public ServerOptions Server {
        get => _server ??= ServerOptions.Default;
        set => _server = value;
    }

    public ExchangeOptions[] Exchanges {
        get => _exchanges ??= [];
        set => _exchanges = value;
    }
}