using Nameless.Mediator.Requests;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Mediator.Pipelines;

[UnitTest]
public class PerformanceRequestPipelineBehaviorTests {
    [Fact]
    public async Task HandleAsync_ExecutesNextDelegate_ReturnsResult() {
        // arrange
        var logger = new LoggerMocker<PerformanceRequestPipelineBehavior<PerformancePipelineTestRequest, string>>()
            .WithAnyLogLevel()
            .Build();

        var sut = new PerformanceRequestPipelineBehavior<PerformancePipelineTestRequest, string>(logger);

        RequestHandlerDelegate<string> next = _ => Task.FromResult("performance-result");

        // act
        var result = await sut.HandleAsync(new PerformancePipelineTestRequest(), next, CancellationToken.None);

        // assert
        Assert.Equal("performance-result", result);
    }

    [Fact]
    public async Task HandleAsync_AlwaysLogsElapsedTime() {
        // arrange
        var loggerMocker = new LoggerMocker<PerformanceRequestPipelineBehavior<PerformancePipelineTestRequest, string>>()
            .WithAnyLogLevel();

        var sut = new PerformanceRequestPipelineBehavior<PerformancePipelineTestRequest, string>(loggerMocker.Build());

        RequestHandlerDelegate<string> next = _ => Task.FromResult("any");

        // act
        await sut.HandleAsync(new PerformancePipelineTestRequest(), next, CancellationToken.None);

        // assert
        // The behavior emits a Starting log and a Finished log, both at Debug level.
        loggerMocker.VerifyDebug(times: 2, exactly: false);
    }
}

// ---------------------------------------------------------------------------
// Test types — declared at namespace level so Castle.DynamicProxy can
// proxy ILogger<T> where T contains this type as a generic argument.
// ---------------------------------------------------------------------------

public record PerformancePipelineTestRequest : IRequest<string>;
