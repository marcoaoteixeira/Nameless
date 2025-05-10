using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Services;
using Shouldly;

namespace Nameless.PubSub.RabbitMQ;

public class DependencyInjectionTests {
    [Test]
    public void WhenRegisterServices_ThenResolveServices() {
        // arrange
        var connectionManagerLogger = Mock.Of<ILogger<ConnectionManager>>();
        var channelFactoryLogger = Mock.Of<ILogger<ChannelFactory>>();
        var publisherLogger = Mock.Of<ILogger<Publisher>>();
        var subscriberLogger = Mock.Of<ILogger<Subscriber>>();
        var clock = Mock.Of<IClock>();

        var services = new ServiceCollection();

        services.AddSingleton(connectionManagerLogger);
        services.AddSingleton(channelFactoryLogger);
        services.AddSingleton(clock);
        services.AddSingleton(publisherLogger);
        services.AddSingleton(subscriberLogger);

        services.RegisterRabbitMQPubSub(_ => { });

        var provider = services.BuildServiceProvider();

        // act & assert
        provider.GetService<IConnectionManager>()
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<ConnectionManager>();

        provider.GetService<IChannelFactory>()
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<ChannelFactory>();

        provider.GetService<IPublisher>()
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<Publisher>();

        provider.GetService<ISubscriber>()
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<Subscriber>();
    }
}