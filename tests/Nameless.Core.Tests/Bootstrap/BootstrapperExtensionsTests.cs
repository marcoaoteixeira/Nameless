using Moq;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;

namespace Nameless;

public class BootstrapperExtensionsTests {
    // --- ExecuteAsync(CancellationToken) ---

    [Fact]
    public async Task ExecuteAsync_WithCancellationTokenOnly_DelegatesToFullSignature() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var mock = new Mock<IBootstrapper>();
        mock.Setup(b => b.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                ct))
            .Returns(Task.CompletedTask);

        // act
        await mock.Object.ExecuteAsync(ct);

        // assert
        mock.Verify(
            b => b.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                ct),
            Times.Once);
    }

    // --- ExecuteAsync(FlowContext, CancellationToken) ---

    [Fact]
    public async Task ExecuteAsync_WithContextAndCancellationToken_DelegatesToFullSignature() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var context = new FlowContext();
        var mock = new Mock<IBootstrapper>();
        mock.Setup(b => b.ExecuteAsync(
                context,
                It.IsAny<IProgress<StepProgress>>(),
                ct))
            .Returns(Task.CompletedTask);

        // act
        await mock.Object.ExecuteAsync(context, ct);

        // assert
        mock.Verify(
            b => b.ExecuteAsync(
                context,
                It.IsAny<IProgress<StepProgress>>(),
                ct),
            Times.Once);
    }

    // --- ExecuteAsync(IProgress, CancellationToken) ---

    [Fact]
    public async Task ExecuteAsync_WithProgressAndCancellationToken_DelegatesToFullSignature() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var progress = new NoopProgress();
        var mock = new Mock<IBootstrapper>();
        mock.Setup(b => b.ExecuteAsync(
                It.IsAny<FlowContext>(),
                progress,
                ct))
            .Returns(Task.CompletedTask);

        // act
        await mock.Object.ExecuteAsync(progress, ct);

        // assert
        mock.Verify(
            b => b.ExecuteAsync(
                It.IsAny<FlowContext>(),
                progress,
                ct),
            Times.Once);
    }

    // --- test doubles ---

    private sealed class NoopProgress : IProgress<StepProgress> {
        public void Report(StepProgress value) { }
    }
}
