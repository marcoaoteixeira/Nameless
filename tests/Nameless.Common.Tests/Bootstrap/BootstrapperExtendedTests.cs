using Moq;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Testing.Tools.Mockers.StatusReporting;

namespace Nameless.Bootstrap;

[UnitTest]
public class BootstrapperExtendedTests {
    // Builds an IStep mock that is disabled (IsEnabled = false).
    private static Mock<IStep> CreateDisabledStep(string name) {
        var mock = new Mock<IStep>();
        mock.Setup(s => s.Name).Returns(name);
        mock.Setup(s => s.DisplayName).Returns(name);
        mock.Setup(s => s.IsEnabled).Returns(false);
        mock.Setup(s => s.Dependencies).Returns([]);
        mock.Setup(s => s.RetryPolicy).Returns(default(RetryPolicyConfiguration));
        return mock;
    }

    private static Mock<IStep> CreateEnabledStep(string name, Task? executionTask = null) {
        var mock = new Mock<IStep>();
        mock.Setup(s => s.Name).Returns(name);
        mock.Setup(s => s.DisplayName).Returns(name);
        mock.Setup(s => s.IsEnabled).Returns(true);
        mock.Setup(s => s.Dependencies).Returns([]);
        mock.Setup(s => s.RetryPolicy).Returns(default(RetryPolicyConfiguration));
        mock.Setup(s => s.ExecuteAsync(
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns(executionTask ?? Task.CompletedTask);
        return mock;
    }

    private static Bootstrapper CreateSut(IEnumerable<IStep> steps) {
        var retryFactoryMock = new Mock<IRetryPipelineFactory>();
        retryFactoryMock
            .Setup(f => f.Create(It.IsAny<RetryPolicyConfiguration>()))
            .Returns(RetryPipeline.Empty);

        return new Bootstrapper(retryFactoryMock.Object,
            new StatusReporterMocker<Bootstrapper>(channelKey: null).Build(),
            steps, TimeProvider.System, new LoggerMocker<Bootstrapper>().WithAnyLogLevel().Build());
    }

    [Fact]
    public async Task ExecuteAsync_WithDisabledStep_DoesNotExecuteStep() {
        // arrange
        var disabledStep = CreateDisabledStep("DisabledStep");
        var sut = CreateSut([disabledStep.Object]);

        // act
        await sut.RunAsync(CancellationToken.None);

        // assert — ExecuteAsync on the step must never be called because IsDisabled = true
        disabledStep.Verify(
            s => s.ExecuteAsync(
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleSuccessfulSteps_CompleteWithoutException() {
        // arrange — two independent steps both succeed
        var stepA = CreateEnabledStep("StepA");
        var stepB = CreateEnabledStep("StepB");
        var sut = CreateSut([stepA.Object, stepB.Object]);

        // act
        var exception = await Record.ExceptionAsync(() =>
            sut.RunAsync(CancellationToken.None)
        );

        // assert
        Assert.Null(exception);

        stepA.Verify(
            s => s.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        stepB.Verify(
            s => s.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMultipleStepsFail_BootstrapExceptionContainsAllFailures() {
        // arrange
        var failA = CreateEnabledStep("FailA", Task.FromException(new InvalidOperationException("error A")));
        var failB = CreateEnabledStep("FailB", Task.FromException(new InvalidOperationException("error B")));
        var sut = CreateSut([failA.Object, failB.Object]);

        // act
        var exception = await Record.ExceptionAsync(() =>
            sut.RunAsync(CancellationToken.None)
        );

        // assert
        var bootstrapEx = Assert.IsType<BootstrapException>(exception);
        Assert.Contains(bootstrapEx.Results, r => r is { Success: false, StepName: "FailA" });
        Assert.Contains(bootstrapEx.Results, r => r is { Success: false, StepName: "FailB" });
    }
}
