using Microsoft.Extensions.Configuration;
using Moq;
using Nameless.ProducerConsumer;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public class ProducerTests {
    private const string Topic = "test.queue";

    private static IConfiguration CreateConfiguration() {
        return ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            [$"RabbitMQ:Queues:{Topic}:Durable"] = "true",
            [$"RabbitMQ:Queues:{Topic}:Exclusive"] = "false",
            [$"RabbitMQ:Queues:{Topic}:AutoDelete"] = "false",
            [$"RabbitMQ:Queues:{Topic}:ExchangeName"] = "test.exchange"
        });
    }

    private static Mock<IChannel> CreateChannelMock() {
        var mock = new Mock<IChannel>(MockBehavior.Loose);

        mock.Setup(c => c.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        return mock;
    }

    private static Mock<IChannelFactory> CreateChannelFactoryMock(IChannel channel) {
        var mock = new Mock<IChannelFactory>(MockBehavior.Strict);

        mock.Setup(f => f.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel);

        return mock;
    }

    private static Mock<IMessageSerializer> CreateSerializerMock(byte[] payload) {
        var mock = new Mock<IMessageSerializer>(MockBehavior.Strict);

        mock.Setup(s => s.SerializeAsync(
                It.IsAny<object>(),
                It.IsAny<Context>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(payload);

        return mock;
    }

    private static Producer CreateSut(
        IChannelFactory channelFactory,
        IConfiguration configuration,
        IMessageSerializer serializer) {

        var logger = new LoggerMocker<Producer>()
            .WithAnyLogLevel()
            .Build();

        return new Producer(channelFactory, configuration, serializer, logger);
    }

    [Fact]
    [UnitTest]
    public async Task ProduceAsync_SerializesMessageAndPublishesToChannel() {
        // arrange
        var payload = new byte[] { 1, 2, 3 };
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var serializerMock = CreateSerializerMock(payload);
        var configuration = CreateConfiguration();
        var sut = CreateSut(factoryMock.Object, configuration, serializerMock.Object);

        var context = new ProducerContext();

        // act
        await sut.ProduceAsync(Topic, message: "hello", context, CancellationToken.None);

        // assert
        serializerMock.Verify(
            s => s.SerializeAsync(
                It.IsAny<object>(),
                It.IsAny<Context>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        channelMock.Verify(
            c => c.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public async Task ProduceAsync_CachesChannelForSameTopic() {
        // arrange
        var payload = new byte[] { 1 };
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var serializerMock = CreateSerializerMock(payload);
        var configuration = CreateConfiguration();
        var sut = CreateSut(factoryMock.Object, configuration, serializerMock.Object);

        var context = new ProducerContext();

        // act — two publishes on the same topic should only create the channel once
        await sut.ProduceAsync(Topic, "msg1", context, CancellationToken.None);
        await sut.ProduceAsync(Topic, "msg2", context, CancellationToken.None);

        // assert
        factoryMock.Verify(
            f => f.CreateAsync(Topic, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public void Dispose_DoesNotThrow() {
        // arrange
        var channelMock = new Mock<IChannel>(MockBehavior.Loose);
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var serializerMock = new Mock<IMessageSerializer>(MockBehavior.Loose);
        var sut = CreateSut(factoryMock.Object, CreateConfiguration(), serializerMock.Object);

        // act
        var exception = Record.Exception(sut.Dispose);

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task DisposeAsync_DoesNotThrow() {
        // arrange
        var channelMock = new Mock<IChannel>(MockBehavior.Loose);
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var serializerMock = new Mock<IMessageSerializer>(MockBehavior.Loose);
        var sut = CreateSut(factoryMock.Object, CreateConfiguration(), serializerMock.Object);

        // act
        var exception = await Record.ExceptionAsync(() => sut.DisposeAsync().AsTask());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task ProduceAsync_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var channelMock = new Mock<IChannel>(MockBehavior.Loose);
        var factoryMock = new Mock<IChannelFactory>(MockBehavior.Loose);
        var serializerMock = new Mock<IMessageSerializer>(MockBehavior.Loose);
        var sut = CreateSut(factoryMock.Object, CreateConfiguration(), serializerMock.Object);

        sut.Dispose();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => sut.ProduceAsync(Topic, "msg", new ProducerContext(), CancellationToken.None)
        );
    }
}
