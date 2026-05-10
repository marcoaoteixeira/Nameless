using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.ProducerConsumer;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.Resilience;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public class ConsumerTests {
    // Concrete consumer used exclusively inside this test file.
    private sealed class TestConsumer : Consumer<string> {
        public override string Name => "test-consumer";
        public override string Topic => "test.queue";

        public TestConsumer(
            IChannelFactory channelFactory,
            IMessageSerializer serializer,
            IRetryPipelineFactory retryPipelineFactory,
            ILogger<Consumer<string>> logger)
            : base(channelFactory, serializer, retryPipelineFactory, logger) { }

        public override Task ConsumeAsync(string message, ConsumerContext context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private static Mock<IChannel> CreateChannelMock() {
        var mock = new Mock<IChannel>(MockBehavior.Loose);

        // Extension BasicConsumeAsync(string, bool, string, IAsyncBasicConsumer, CT) delegates
        // to the full 8-parameter interface method.
        mock.Setup(c => c.BasicConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("consumer-tag");

        mock.Setup(c => c.CloseAsync(
                It.IsAny<ushort>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    private static Mock<IChannelFactory> CreateChannelFactoryMock(IChannel channel) {
        var mock = new Mock<IChannelFactory>(MockBehavior.Strict);

        mock.Setup(f => f.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel);

        return mock;
    }

    private static Mock<IMessageSerializer> CreateSerializerMock() {
        var mock = new Mock<IMessageSerializer>(MockBehavior.Loose);
        return mock;
    }

    private static Mock<IRetryPipelineFactory> CreateRetryPipelineFactoryMock() {
        var pipelineMock = new Mock<IRetryPipeline>(MockBehavior.Loose);

        pipelineMock
            .Setup(p => p.ExecuteAsync(
                It.IsAny<Func<CancellationToken, ValueTask>>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var factoryMock = new Mock<IRetryPipelineFactory>(MockBehavior.Loose);

        factoryMock
            .Setup(f => f.Create(It.IsAny<RetryPolicyConfiguration>()))
            .Returns(pipelineMock.Object);

        return factoryMock;
    }

    private static TestConsumer CreateSut(IChannelFactory channelFactory) {
        var serializer = CreateSerializerMock();
        var retryFactory = CreateRetryPipelineFactoryMock();

        var logger = new LoggerMocker<Consumer<string>>()
            .WithAnyLogLevel()
            .Build();

        return new TestConsumer(channelFactory, serializer.Object, retryFactory.Object, logger);
    }

    [Fact]
    [UnitTest]
    public async Task StartAsync_SetsUpConsumer_DoesNotThrow() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act
        var exception = await Record.ExceptionAsync(
            () => ((IHostedService)sut).StartAsync(CancellationToken.None)
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task StartAsync_RegistersConsumerWithChannel() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act
        await ((IHostedService)sut).StartAsync(CancellationToken.None);

        // assert — the full interface method must have been called exactly once
        channelMock.Verify(
            c => c.BasicConsumeAsync(
                sut.Topic,
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public async Task StopAsync_WhenStarted_ClosesChannel() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        await ((IHostedService)sut).StartAsync(CancellationToken.None);

        // act
        await ((IHostedService)sut).StopAsync(CancellationToken.None);

        // assert
        channelMock.Verify(
            c => c.CloseAsync(
                It.IsAny<ushort>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [UnitTest]
    public void Dispose_DoesNotThrow() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act
        var exception = Record.Exception(sut.Dispose);

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task DisposeAsync_DoesNotThrow() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act
        var exception = await Record.ExceptionAsync(() => sut.DisposeAsync().AsTask());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    [UnitTest]
    public async Task StartAsync_AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        sut.Dispose();

        // act & assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => ((IHostedService)sut).StartAsync(CancellationToken.None)
        );
    }

    [Fact]
    [UnitTest]
    public void TestConsumer_Name_ReturnsExpectedValue() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act & assert
        Assert.Equal("test-consumer", sut.Name);
    }

    [Fact]
    [UnitTest]
    public void TestConsumer_Topic_ReturnsExpectedValue() {
        // arrange
        var channelMock = CreateChannelMock();
        var factoryMock = CreateChannelFactoryMock(channelMock.Object);
        var sut = CreateSut(factoryMock.Object);

        // act & assert
        Assert.Equal("test.queue", sut.Topic);
    }
}
