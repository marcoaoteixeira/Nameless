using Microsoft.Extensions.Options;
using Moq;
using Nameless.Bootstrap.Notification;
using Nameless.Resilience;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;
using Nameless.Testing.Tools.Mockers.System;

namespace Nameless.Bootstrap;

[UnitTest]
public class ParallelBootstrapperTests {
    private static Mock<IStep> CreateEnabledStep(string name, string[]? dependencies = null, Task? executionTask = null) {
        var mock = new Mock<IStep>();
        mock.Setup(s => s.Name).Returns(name);
        mock.Setup(s => s.DisplayName).Returns(name);
        mock.Setup(s => s.IsEnabled).Returns(true);
        mock.Setup(s => s.Dependencies).Returns(dependencies ?? []);
        mock.Setup(s => s.RetryPolicy).Returns(default(RetryPolicyConfiguration));
        mock.Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns(executionTask ?? Task.CompletedTask);
        return mock;
    }

    private static ParallelBootstrapper CreateSut(IEnumerable<IStep> steps, BootstrapOptions? options = null) {
        var retryFactoryMock = new Mock<IRetryPipelineFactory>();
        retryFactoryMock
            .Setup(f => f.Create(It.IsAny<RetryPolicyConfiguration>()))
            .Returns(RetryPipeline.Empty);

        var logger = new LoggerMocker<ParallelBootstrapper>().WithAnyLogLevel().Build();
        var bootstrapperLogger = new LoggerMocker<Bootstrapper>().WithAnyLogLevel().Build();

        var bootstrapOptions = Options.Create(options ?? new BootstrapOptions {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = -1
        });

        return new ParallelBootstrapper(
            steps,
            retryFactoryMock.Object,
            TimeProvider.System,
            logger,
            bootstrapOptions
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleIndependentSteps_AllStepsExecuted() {
        // arrange
        var stepA = CreateEnabledStep("StepA");
        var stepB = CreateEnabledStep("StepB");
        var stepC = CreateEnabledStep("StepC");

        var sut = CreateSut([stepA.Object, stepB.Object, stepC.Object]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        await sut.ExecuteAsync(context, progress, CancellationToken.None);

        // assert
        stepA.Verify(
            s => s.ExecuteAsync(context, It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        stepB.Verify(
            s => s.ExecuteAsync(context, It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        stepC.Verify(
            s => s.ExecuteAsync(context, It.IsAny<IProgress<StepProgress>>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithDependentSteps_ExecutesInCorrectOrder() {
        // arrange
        // StepA has no deps; StepB depends on StepA
        var executionOrder = new List<string>();

        var stepA = CreateEnabledStep("StepA");
        stepA.Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns(() => {
                executionOrder.Add("StepA");
                return Task.CompletedTask;
            });

        var stepB = CreateEnabledStep("StepB", dependencies: ["StepA"]);
        stepB.Setup(s => s.ExecuteAsync(
                It.IsAny<FlowContext>(),
                It.IsAny<IProgress<StepProgress>>(),
                It.IsAny<CancellationToken>()))
            .Returns(() => {
                executionOrder.Add("StepB");
                return Task.CompletedTask;
            });

        var sut = CreateSut([stepA.Object, stepB.Object]);
        var context = new FlowContext();
        var progress = new ProgressMocker<StepProgress>().Build();

        // act
        await sut.ExecuteAsync(context, progress, CancellationToken.None);

        // assert
        Assert.Equal(["StepA", "StepB"], executionOrder);
    }
}
