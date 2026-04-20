using Moq;
using Nameless.ProducerConsumer;

namespace Nameless;

public class ProducerConsumerExtensionsTests {
    // --- ConsumerExtensions.HandleAsync ---

    [Fact]
    public async Task HandleAsync_DelegatesWithEmptyContext() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var mock = new Mock<IConsumer<string>>();
        mock.Setup(c => c.ConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<ConsumerContext>(),
                ct))
            .Returns(Task.CompletedTask);

        // act
        await mock.Object.HandleAsync("message", ct);

        // assert
        mock.Verify(
            c => c.ConsumeAsync("message", It.IsAny<ConsumerContext>(), ct),
            Times.Once);
    }

    // --- ProducerExtensions.ProduceAsync ---

    [Fact]
    public async Task ProduceAsync_DelegatesWithEmptyContext() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var mock = new Mock<IProducer>();
        mock.Setup(p => p.ProduceAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<ProducerContext>(),
                ct))
            .Returns(Task.CompletedTask);

        // act
        await mock.Object.ProduceAsync("orders", new { Id = 1 }, ct);

        // assert
        mock.Verify(
            p => p.ProduceAsync("orders", It.IsAny<object>(), It.IsAny<ProducerContext>(), ct),
            Times.Once);
    }
}
