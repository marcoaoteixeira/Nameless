using Moq;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

[UnitTest]
public class ProducerTests {
    private const string Topic = "test.queue";

    private static (Producer Sut, Mock<IChannel> Channel, Mock<IChannelFactory> Factory) CreateSut() {
        var channel = new Mock<IChannel>();
        channel.Setup(c => c.BasicPublishAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var factory = new Mock<IChannelFactory>();
        factory.Setup(f => f.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(channel.Object);

        var options = Microsoft.Extensions.Options.Options.Create(new RabbitMQOptions {
            Queues = [new QueueOptions { Name = Topic, ExchangeName = "ex" }]
        });

        var logger = new LoggerMocker<Producer>().WithAnyLogLevel().Build();

        return (new Producer(factory.Object, new JsonMessageSerializer(), options, logger), channel, factory);
    }

    [Fact]
    public async Task ProduceAsync_PublishesToConfiguredExchangeAndTopic() {
        // arrange
        var (sut, channel, _) = CreateSut();
        var context = new ProducerContext { Mandatory = true };

        // act
        await sut.ProduceAsync(Topic, "hello", context, TestContext.Current.CancellationToken);

        // assert
        channel.Verify(c => c.BasicPublishAsync(
            "ex", Topic, true, It.IsAny<BasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProduceAsync_CalledTwice_ReusesChannel() {
        // arrange
        var (sut, _, factory) = CreateSut();

        // act
        await sut.ProduceAsync(Topic, "a", new ProducerContext(), TestContext.Current.CancellationToken);
        await sut.ProduceAsync(Topic, "b", new ProducerContext(), TestContext.Current.CancellationToken);

        // assert
        factory.Verify(f => f.CreateAsync(Topic, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProduceAsync_WithUnknownTopic_ThrowsMissingQueueConfigurationException() {
        // arrange
        var (sut, _, _) = CreateSut();

        // act & assert
        await Assert.ThrowsAsync<MissingQueueConfigurationException>(
            () => sut.ProduceAsync("unknown", "a", new ProducerContext(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ProduceAsync_WhenPublishFails_Rethrows() {
        // arrange
        var (sut, channel, _) = CreateSut();
        channel.Setup(c => c.BasicPublishAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("broker"));

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ProduceAsync(Topic, "a", new ProducerContext(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ProduceAsync_WithCancelledToken_Throws() {
        // arrange
        var (sut, _, _) = CreateSut();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // act & assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.ProduceAsync(Topic, "a", new ProducerContext(), cts.Token));
    }

    [Fact]
    public async Task ProduceAsync_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var (sut, _, _) = CreateSut();
        sut.Dispose();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => sut.ProduceAsync(Topic, "a", new ProducerContext(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Dispose_DisposesCachedChannelsOnce() {
        // arrange
        var (sut, channel, _) = CreateSut();
        await sut.ProduceAsync(Topic, "a", new ProducerContext(), TestContext.Current.CancellationToken);

        // act
        sut.Dispose();
        sut.Dispose();

        // assert
        channel.Verify(c => c.Dispose(), Times.Once);
    }

    [Fact]
    public async Task DisposeAsync_DisposesCachedChannelsOnce() {
        // arrange
        var (sut, channel, _) = CreateSut();
        await sut.ProduceAsync(Topic, "a", new ProducerContext(), TestContext.Current.CancellationToken);

        // act
        await sut.DisposeAsync();
        await sut.DisposeAsync();

        // assert
        channel.Verify(c => c.DisposeAsync(), Times.Once);
    }
}
