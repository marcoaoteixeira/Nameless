using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Mockers;
using Nameless.PubSub.RabbitMQ.Options;

namespace Nameless.PubSub.RabbitMQ.Utils;
public class SimpleHost : IDisposable {
    private ServiceProvider _provider;

    public IVerifiable<ILogger<ConnectionManager>> ConnectionManagerLogger { get; }
    public IVerifiable<ILogger<ChannelFactory>> ChannelFactoryLogger { get; }
    public IVerifiable<ILogger<Publisher>> PublisherLogger { get; }
    public IVerifiable<ILogger<Subscriber>> SubscriberLogger { get; }

    public IConnectionManager ConnectionManager { get; }
    public IChannelFactory ChannelFactory { get; set; }
    public IPublisher Publisher { get; set; }
    public ISubscriber Subscriber { get; set; }

    public SimpleHost(Action<RabbitMQOptions> configure) {
        var connectionManagerLoggerMocker = new LoggerMocker<ConnectionManager>().WithAllLogLevels();
        var channelFactoryLoggerMocker = new LoggerMocker<ChannelFactory>().WithAllLogLevels();
        var publisherLoggerMocker = new LoggerMocker<Publisher>().WithAllLogLevels();
        var subscriberLoggerMocker = new LoggerMocker<Subscriber>().WithAllLogLevels();

        ConnectionManagerLogger = connectionManagerLoggerMocker;
        ChannelFactoryLogger = channelFactoryLoggerMocker;
        PublisherLogger = publisherLoggerMocker;
        SubscriberLogger = subscriberLoggerMocker;
        
        var services = new ServiceCollection();

        services.AddSingleton(connectionManagerLoggerMocker.Build());
        services.AddSingleton(channelFactoryLoggerMocker.Build());
        services.AddSingleton(publisherLoggerMocker.Build());
        services.AddSingleton(subscriberLoggerMocker.Build());
        services.AddSingleton<IClock>(new Clock());

        services.RegisterRabbitMQPubSub(configure);

        _provider = services.BuildServiceProvider();

        ConnectionManager = _provider.GetService<IConnectionManager>();
        ChannelFactory = _provider.GetService<IChannelFactory>();
        Publisher = _provider.GetService<IPublisher>();
        Subscriber = _provider.GetService<ISubscriber>();
    }

    public void Dispose() {
        _provider?.Dispose();
        _provider = null;
    }
}
