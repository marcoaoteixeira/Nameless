using Microsoft.Extensions.Configuration;
using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class ChannelFactoryTests {
    private const string QUEUE_NAME = "test.queue";

    private static IConfiguration CreateConfiguration(bool prefetchEnabled = false) {
        return ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            // queue section: RabbitMQ > Queues > <queueName>
            [$"RabbitMQ:Queues:{QUEUE_NAME}:Durable"] = "true",
            [$"RabbitMQ:Queues:{QUEUE_NAME}:Exclusive"] = "false",
            [$"RabbitMQ:Queues:{QUEUE_NAME}:AutoDelete"] = "false",
            [$"RabbitMQ:Queues:{QUEUE_NAME}:ExchangeName"] = "test.exchange",
            ["RabbitMQ:Prefetch:IsEnabled"] = prefetchEnabled ? "true" : "false",
            ["RabbitMQ:Prefetch:Count"] = "1",
            ["RabbitMQ:Prefetch:Size"] = "0",
            ["RabbitMQ:Prefetch:Global"] = "false"
        });
    }

    private static Mock<IChannel> CreateChannelMock() {
        var channelMock = new Mock<IChannel>(MockBehavior.Loose);

        // QueueDeclareAsync must return a QueueDeclareOk with a non-empty QueueName
        channelMock
            .Setup(c => c.QueueDeclareAsync(
                QUEUE_NAME,
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueDeclareOk(QUEUE_NAME, messageCount: 0, consumerCount: 0));

        return channelMock;
    }

    private static Mock<IConnection> CreateConnectionMock(Mock<IChannel> channelMock) {
        var connectionMock = new Mock<IConnection>(MockBehavior.Loose);

        connectionMock
            .Setup(c => c.CreateChannelAsync(
                It.IsAny<CreateChannelOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(channelMock.Object);

        return connectionMock;
    }

    private static Mock<IConnectionManager> CreateConnectionManagerMock(Mock<IConnection> connectionMock) {
        var managerMock = new Mock<IConnectionManager>(MockBehavior.Strict);

        managerMock
            .Setup(m => m.GetConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionMock.Object);

        return managerMock;
    }

    [Fact]
    [UnitTest]
    public async Task CreateAsync_ReturnsChannel() {
        // arrange
        var channelMock = CreateChannelMock();
        var connectionMock = CreateConnectionMock(channelMock);
        var managerMock = CreateConnectionManagerMock(connectionMock);
        var configuration = CreateConfiguration();

        var sut = new ChannelFactory(configuration, managerMock.Object);

        // act
        var channel = await sut.CreateAsync(QUEUE_NAME, CancellationToken.None);

        // assert
        Assert.NotNull(channel);
        Assert.Same(channelMock.Object, channel);
    }

    [Fact]
    [UnitTest]
    public async Task CreateAsync_DeclaresQueue() {
        // arrange
        var channelMock = CreateChannelMock();
        var connectionMock = CreateConnectionMock(channelMock);
        var managerMock = CreateConnectionManagerMock(connectionMock);
        var configuration = CreateConfiguration();

        var sut = new ChannelFactory(configuration, managerMock.Object);

        // act
        await sut.CreateAsync(QUEUE_NAME, CancellationToken.None);

        // assert
        channelMock.Verify(
            c => c.QueueDeclareAsync(
                QUEUE_NAME,
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public async Task CreateAsync_WithPrefetchEnabled_CallsBasicQos() {
        // arrange
        var channelMock = CreateChannelMock();

        channelMock
            .Setup(c => c.BasicQosAsync(
                It.IsAny<uint>(),
                It.IsAny<ushort>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var connectionMock = CreateConnectionMock(channelMock);
        var managerMock = CreateConnectionManagerMock(connectionMock);
        var configuration = CreateConfiguration(prefetchEnabled: true);

        var sut = new ChannelFactory(configuration, managerMock.Object);

        // act
        await sut.CreateAsync(QUEUE_NAME, CancellationToken.None);

        // assert
        channelMock.Verify(
            c => c.BasicQosAsync(
                It.IsAny<uint>(),
                It.IsAny<ushort>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public async Task CreateAsync_WithPrefetchDisabled_DoesNotCallBasicQos() {
        // arrange
        var channelMock = CreateChannelMock();
        var connectionMock = CreateConnectionMock(channelMock);
        var managerMock = CreateConnectionManagerMock(connectionMock);
        var configuration = CreateConfiguration(prefetchEnabled: false);

        var sut = new ChannelFactory(configuration, managerMock.Object);

        // act
        await sut.CreateAsync(QUEUE_NAME, CancellationToken.None);

        // assert
        channelMock.Verify(
            c => c.BasicQosAsync(
                It.IsAny<uint>(),
                It.IsAny<ushort>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
