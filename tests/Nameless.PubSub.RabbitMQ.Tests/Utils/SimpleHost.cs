#pragma warning disable S3881 // Test class, we don't need to perfectly implement IDisposable here.

using Microsoft.Extensions.DependencyInjection;
using Nameless.PubSub.RabbitMQ.Infrastructure;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.PubSub.RabbitMQ.Utils;

public class SimpleHost : IDisposable {
    private bool _disposed;
    private ServiceProvider _provider;

    public LoggerMocker<ConnectionManager> ConnectionManagerLoggerMocker { get; }
    public LoggerMocker<ChannelFactory> ChannelFactoryLoggerMocker { get; }
    public LoggerMocker<Publisher> PublisherLoggerMocker { get; }
    public LoggerMocker<Subscriber> SubscriberLoggerMocker { get; }

    public IConnectionManager ConnectionManager { get; }
    public IChannelFactory ChannelFactory { get; set; }
    public IPublisher Publisher { get; set; }
    public ISubscriber Subscriber { get; set; }

    public SimpleHost(Action<RabbitMQOptions> configure) {
        ConnectionManagerLoggerMocker = new LoggerMocker<ConnectionManager>().EnableAllLogLevels();
        ChannelFactoryLoggerMocker = new LoggerMocker<ChannelFactory>().EnableAllLogLevels();
        PublisherLoggerMocker = new LoggerMocker<Publisher>().EnableAllLogLevels();
        SubscriberLoggerMocker = new LoggerMocker<Subscriber>().EnableAllLogLevels();

        var services = new ServiceCollection();

        services.AddSingleton(ConnectionManagerLoggerMocker.Build());
        services.AddSingleton(ChannelFactoryLoggerMocker.Build());
        services.AddSingleton(PublisherLoggerMocker.Build());
        services.AddSingleton(SubscriberLoggerMocker.Build());
        services.AddSingleton(TimeProvider.System);

        services.RegisterPubSubServices(configure);

        _provider = services.BuildServiceProvider();

        ConnectionManager = _provider.GetService<IConnectionManager>();
        ChannelFactory = _provider.GetService<IChannelFactory>();
        Publisher = _provider.GetService<IPublisher>();
        Subscriber = _provider.GetService<ISubscriber>();
    }

    public void Dispose() {
        if (_disposed) { return; }

        _provider?.Dispose();
        _provider = null;
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}