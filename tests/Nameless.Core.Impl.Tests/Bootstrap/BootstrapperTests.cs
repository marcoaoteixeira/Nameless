using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Testing.Tools.Mockers.System;

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
        mock.Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
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

        return new Bootstrapper(
            steps,
            retryFactoryMock.Object,
            TimeProvider.System,
            new LoggerMocker<Bootstrapper>().WithAnyLogLevel().Build()
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithNoSteps_CompletesSuccessfully() {
        // arrange
        var sut = CreateSut([]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        var exception = await Record.ExceptionAsync(() =>
            sut.ExecuteAsync(context, progress, CancellationToken.None)
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ExecuteAsync_WithOneStep_ExecutesStep() {
        // arrange
        var stepMock = CreateEnabledStep("OnlyStep");
        var sut = CreateSut([stepMock.Object]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        await sut.ExecuteAsync(context, progress, CancellationToken.None);

        // assert
        stepMock.Verify(
            s => s.ExecuteAsync(context, It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenStepThrows_RecordsFailureButContinues() {
        // arrange
        var failingStep = CreateEnabledStep(
            "FailingStep",
            Task.FromException(new InvalidOperationException("boom"))
        );
        var succeedingStep = CreateEnabledStep("SucceedingStep");

        var sut = CreateSut([failingStep.Object, succeedingStep.Object]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        // Bootstrapper catches step exceptions internally but throws BootstrapException after all steps run
        var exception = await Record.ExceptionAsync(() =>
            sut.ExecuteAsync(context, progress, CancellationToken.None)
        );

        // assert
        var bootstrapEx = Assert.IsType<BootstrapException>(exception);
        Assert.Contains(bootstrapEx.Results, r => !r.Success && r.StepName == "FailingStep");

        // The succeeding step still ran despite the preceding failure
        succeedingStep.Verify(
            s => s.ExecuteAsync(context, It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationRequested_StopsExecution() {
        // arrange
        using var cts = new CancellationTokenSource();

        // The first step cancels the token when it executes; the second step (in a later
        // dependency level) will observe the cancellation and throw OperationCanceledException.
        var firstStep = CreateEnabledStep("FirstStep");
        firstStep
            .Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns<FlowContext, IProgress<StepProgress>, CancellationToken>((_, _, _) => {
                cts.Cancel();
                return Task.CompletedTask;
            });

        // Second step depends on the first so it runs in a later level after cancellation
        var secondStep = CreateEnabledStep("SecondStep");
        secondStep.Setup(s => s.Dependencies).Returns(["FirstStep"]);
        secondStep
            .Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns<FlowContext, IProgress<StepProgress>, CancellationToken>((_, _, token) => {
                token.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            });

        var sut = CreateSut([firstStep.Object, secondStep.Object]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        var exception = await Record.ExceptionAsync(() =>
            sut.ExecuteAsync(context, progress, cts.Token)
        );

        // assert
        // The bootstrapper records the OperationCanceledException on the second step's result
        // and then surfaces it as a BootstrapException once all levels have been visited.
        var bootstrapEx = Assert.IsType<BootstrapException>(exception);
        Assert.Contains(bootstrapEx.Results, r => !r.Success && r.StepName == "SecondStep");
    }
}
