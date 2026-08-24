using Moq;

namespace Nameless.Bootstrap;

public class BootstrapperExtensionsTests {
    [Fact]
    public async Task WhenExecuteAsync_WithCancellationTokenOnly_ThenDelegatesToFullSignature() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var bootstrapperMock = new Mock<IBootstrapper>();

        bootstrapperMock.Setup(mock => mock.RunAsync(ct))
                        .Returns(Task.CompletedTask);

        // act
        await bootstrapperMock.Object.RunAsync(ct);

        // assert
        bootstrapperMock.Verify(mock => mock.RunAsync(ct), Times.Once);
    }
}
