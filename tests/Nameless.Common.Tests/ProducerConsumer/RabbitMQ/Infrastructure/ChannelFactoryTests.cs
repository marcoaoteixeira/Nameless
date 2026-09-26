using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

[UnitTest]
public class ChannelFactoryTests {
    private const string QueueName = "test.queue";

    private static (ChannelFactory Sut, Mock<IChannel> Channel) CreateSut(QueueOptions? queue = null, string declaredName = QueueName) {
        var channel = new Mock<IChannel>();
        channel.Setup(c => c.QueueDeclareAsync(
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueDeclareOk(declaredName, 0, 0));

        var connection = new Mock<IConnection>();
        connection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(channel.Object);

        var connectionManager = new Mock<IConnectionManager>();
        connectionManager.Setup(m => m.GetConnectionAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(connection.Object);

        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions {
            Queues = [queue ?? new QueueOptions { Name = QueueName, Durable = true, ExchangeName = "ex" }]
        });

        return (new ChannelFactory(options, connectionManager.Object), channel);
    }

    [Fact]
    public async Task CreateAsync_DeclaresQueueWithConfiguredSettings() {
        // arrange
        var (sut, channel) = CreateSut(new QueueOptions {
            Name = QueueName, Durable = true, Exclusive = true, AutoDelete = true
        });

        // act
        var actual = await sut.CreateAsync(QueueName, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(actual);
        channel.Verify(c => c.QueueDeclareAsync(
            QueueName, true, true, true, It.IsAny<IDictionary<string, object?>>(), false, false,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_BindsEveryConfiguredBinding() {
        // arrange
        var (sut, channel) = CreateSut(new QueueOptions {
            Name = QueueName,
            ExchangeName = "ex",
            Bindings = [new BindingOptions { RoutingKey = "a" }, new BindingOptions { RoutingKey = "b" }]
        });

        // act
        await sut.CreateAsync(QueueName, TestContext.Current.CancellationToken);

        // assert
        channel.Verify(c => c.QueueBindAsync(
            QueueName, "ex", It.IsIn("a", "b"), It.IsAny<IDictionary<string, object?>>(), false,
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_WithPrefetchEnabled_ConfiguresQos() {
        // arrange
        var (sut, channel) = CreateSut(new QueueOptions {
            Name = QueueName,
            Prefetch = new PrefetchOptions { IsEnabled = true, Size = 10, Count = 5, Global = true }
        });

        // act
        await sut.CreateAsync(QueueName, TestContext.Current.CancellationToken);

        // assert
        channel.Verify(c => c.BasicQosAsync(10u, (ushort)5, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithPrefetchDisabled_DoesNotConfigureQos() {
        // arrange
        var (sut, channel) = CreateSut(new QueueOptions {
            Name = QueueName, Prefetch = new PrefetchOptions { IsEnabled = false }
        });

        // act
        await sut.CreateAsync(QueueName, TestContext.Current.CancellationToken);

        // assert
        channel.Verify(c => c.BasicQosAsync(It.IsAny<uint>(), It.IsAny<ushort>(), It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownQueue_ThrowsMissingQueueConfigurationException() {
        // arrange
        var (sut, _) = CreateSut();

        // act
        var exception = await Assert.ThrowsAsync<MissingQueueConfigurationException>(
            () => sut.CreateAsync("unknown", TestContext.Current.CancellationToken));

        // assert
        Assert.Equal("unknown", exception.QueueName);
    }

    [Fact]
    public async Task CreateAsync_WithBlankQueueName_Throws() {
        // arrange
        var (sut, _) = CreateSut();

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateAsync(" ", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_WhenDeclareReturnsBlankName_ThrowsInvalidOperationException() {
        // arrange
        var (sut, _) = CreateSut(declaredName: "");

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateAsync(QueueName, TestContext.Current.CancellationToken));
    }
}
