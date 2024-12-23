namespace Nameless.PubSub.RabbitMQ.Options;

public sealed record RabbitMQOptions {
    public TimeSpan ConsumerStartupTimeout { get; set; } = TimeSpan.FromSeconds(1);

    private ServerSettings? _server;
    public ServerSettings Server {
        get => _server ??= new ServerSettings();
        set => _server = value;
    }

    private ExchangeSettings[]? _exchanges;
    public ExchangeSettings[] Exchanges {
        get => _exchanges ??= [];
        set => _exchanges = value;
    }
}