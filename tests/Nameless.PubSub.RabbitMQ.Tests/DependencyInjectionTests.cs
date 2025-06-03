using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.PubSub.RabbitMQ.Infrastructure;

namespace Nameless.PubSub.RabbitMQ;

public class DependencyInjectionTests {
    [Test]
    public void WhenRegisterServices_ThenResolveServices() {
        // arrange
        var connectionManagerLogger = Mock.Of<ILogger<ConnectionManager>>();
        var channelFactoryLogger = Mock.Of<ILogger<ChannelFactory>>();
        var publisherLogger = Mock.Of<ILogger<Publisher>>();
        var subscriberLogger = Mock.Of<ILogger<Subscriber>>();

        var services = new ServiceCollection();

        services.AddSingleton(connectionManagerLogger);
        services.AddSingleton(channelFactoryLogger);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(publisherLogger);
        services.AddSingleton(subscriberLogger);

        services.RegisterPubSubServices(_ => { });

        // act & assert
        Assert.Multiple(() => {
            var provider = services.BuildServiceProvider();

            var connectionManager = provider.GetService<IConnectionManager>();
            Assert.That(connectionManager, Is.Not.Null);
            Assert.That(connectionManager, Is.InstanceOf<ConnectionManager>());

            var channelFactory = provider.GetService<IChannelFactory>();
            Assert.That(channelFactory, Is.Not.Null);
            Assert.That(channelFactory, Is.InstanceOf<ChannelFactory>());

            var publisher = provider.GetService<IPublisher>();
            Assert.That(publisher, Is.Not.Null);
            Assert.That(publisher, Is.InstanceOf<Publisher>());

            var subscriber = provider.GetService<ISubscriber>();
            Assert.That(subscriber, Is.Not.Null);
            Assert.That(subscriber, Is.InstanceOf<Subscriber>());
        });
    }
}