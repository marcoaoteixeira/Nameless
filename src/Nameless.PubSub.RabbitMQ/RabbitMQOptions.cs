namespace Nameless.PubSub.RabbitMQ;

public sealed record RabbitMQOptions {
    private ExchangeSettings[]? _exchanges;

    private ServerSettings? _server;
    public TimeSpan ConsumerStartupTimeout { get; set; } = TimeSpan.FromSeconds(1);

    public ServerSettings Server {
        get => _server ??= new ServerSettings();
        set => _server = value;
    }

    public ExchangeSettings[] Exchanges {
        get => _exchanges ??= [];
        set => _exchanges = value;
    }
}