using Moq;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Testing.Tools.Mockers.StatusReporting;

namespace Nameless.Bootstrap;

[UnitTest]
public class BootstrapperTests {
    // Builds a loose IStep mock that is enabled by default.
    private static Mock<IStep> CreateEnabledStep(string name, Task? executionTask = null) {
        var mock = new Mock<IStep>();
        mock.Setup(s => s.Name).Returns(name);
        mock.Setup(s => s.DisplayName).Returns(name);
        mock.Setup(s => s.IsEnabled).Returns(true);
        mock.Setup(s => s.Dependencies).Returns([]);
        mock.Setup(s => s.RetryPolicy).Returns(default(RetryPolicyConfiguration));
        mock.Setup(s => s.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()))
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
    public async Task ExecuteAsync_WithNoSteps_CompletesSuccessfully() {
        // arrange
        var sut = CreateSut([]);

        // act
        var exception = await Record.ExceptionAsync(() => sut.RunAsync(CancellationToken.None));

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ExecuteAsync_WithOneStep_ExecutesStep() {
        // arrange
        var stepMocker = CreateEnabledStep("OnlyStep");
        var sut = CreateSut([stepMocker.Object]);
        
        // act
        await sut.RunAsync(CancellationToken.None);

        // assert
        stepMocker.Verify(mock => mock.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStepThrows_RecordsFailureButContinues() {
        // arrange
        var failingStep = CreateEnabledStep(
            "FailingStep",
            Task.FromException(new InvalidOperationException("boom"))
        );
        var succeedingStepMocker = CreateEnabledStep("SucceedingStep");

        var sut = CreateSut([failingStep.Object, succeedingStepMocker.Object]);

        // act
        // Bootstrapper catches step exceptions internally but throws BootstrapException after all steps run
        var exception = await Record.ExceptionAsync(() => sut.RunAsync(CancellationToken.None));

        // assert
        var bootstrapEx = Assert.IsType<BootstrapException>(exception);
        Assert.Contains(bootstrapEx.Results, results => results is { Success: false, StepName: "FailingStep" });

        // The succeeding step still ran despite the preceding failure
        succeedingStepMocker.Verify(mock => mock.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationRequested_StopsExecution() {
        // arrange
        using var cts = new CancellationTokenSource();

        // The first step cancels the token when it executes; the second step (in a later
        // dependency level) will observe the cancellation and throw OperationCanceledException.
        var firstStepMocker = CreateEnabledStep("FirstStep");
        firstStepMocker
            .Setup(mock => mock.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(_ => {
                // ReSharper disable once AccessToDisposedClosure
                cts.Cancel();

                return Task.CompletedTask;
            });

        // Second step depends on the first so it runs in a later level after cancellation
        var secondStepMocker = CreateEnabledStep("SecondStep");
        
        secondStepMocker.Setup(mock => mock.Dependencies)
                        .Returns(["FirstStep"]);

        secondStepMocker.Setup(mock => mock.ExecuteAsync(It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()))
                        .Returns<CancellationToken>(token => {
                            token.ThrowIfCancellationRequested();
                            return Task.CompletedTask;
                        });

        var sut = CreateSut([firstStepMocker.Object, secondStepMocker.Object]);
        
        // act
        var exception = await Record.ExceptionAsync(() => sut.RunAsync(cts.Token));

        // assert
        // The bootstrapper records the OperationCanceledException on the second step's result
        // and then surfaces it as a BootstrapException once all levels have been visited.
        var bootstrapEx = Assert.IsType<BootstrapException>(exception);
        Assert.Contains(
            bootstrapEx.Results,
            results => results is { Success: false, StepName: "SecondStep" }
        );
    }
}
