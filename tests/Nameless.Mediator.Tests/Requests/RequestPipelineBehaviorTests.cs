using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Mediator.Fixtures.Requests;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Requests;

public class RequestPipelineBehaviorTests {
    [Fact]
    public async Task WhenRequestHasPipelineBehavior_ThenBehaviorShouldBeExecutedBeforeRequestHandler() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediatorServices(options => {
            options.Assemblies = [typeof(RequestPipelineBehaviorTests).Assembly];
            options.RegisterRequestPipelineBehavior(typeof(LoggerRequestPipelineBehavior<,>));
        });

        var loggerMocker = new LoggerMocker<object>().EnableAllLogLevels();
        services.AddTransient(_ => (ILogger)loggerMocker.Build());

        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        var request = new RequestWithResponse { ReturnValue = 123 };
        var response = await mediator.ExecuteAsync(request, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(response);
            loggerMocker.VerifyInformationCall(times: Times.Once());
        });
    }

    [Fact]
    public async Task WhenRequestHasPipelineBehavior_ThenBehaviorShouldBeExecutedForSpecificRequest() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediatorServices(options => {
            options.Assemblies = [typeof(RequestPipelineBehaviorTests).Assembly];
            options.RegisterRequestPipelineBehavior(typeof(LoggerRequestPipelineBehavior<RequestWithoutResponse, Nothing>));
        });

        var loggerMocker = new LoggerMocker<object>().EnableAllLogLevels();
        services.AddTransient(_ => (ILogger)loggerMocker.Build());
        services.AddTransient(_ => Quick.Mock<ILogger<RequestWithoutResponseRequestHandler>>());

        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        var requestWithResponse = new RequestWithResponse { ReturnValue = 123 };
        var responseForRequestWithResponse = await mediator.ExecuteAsync(requestWithResponse, CancellationToken.None);

        var requestWithoutResponse = new RequestWithoutResponse { Value = 123 };
        await mediator.ExecuteAsync(requestWithoutResponse, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(responseForRequestWithResponse);
            loggerMocker.VerifyInformationCall(times: Times.Once());
        });
    }

    [Fact]
    public async Task WhenOpenGenericPipelineBehavior_WhenMultipleRequestHandler_ThenExecutePipelineForEachRequestHandler() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediatorServices(options => {
            options.Assemblies = [typeof(RequestPipelineBehaviorTests).Assembly];
            options.RegisterRequestPipelineBehavior(typeof(LoggerRequestPipelineBehavior<,>));
        });

        var loggerMocker = new LoggerMocker<object>().EnableAllLogLevels();
        services.AddTransient(_ => (ILogger)loggerMocker.Build());
        services.AddTransient(_ => Quick.Mock<ILogger<RequestWithoutResponseRequestHandler>>());

        await using var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        var requestWithResponse = new RequestWithResponse { ReturnValue = 123 };
        var responseForRequestWithResponse = await mediator.ExecuteAsync(requestWithResponse, CancellationToken.None);

        var requestWithoutResponse = new RequestWithoutResponse { Value = 123 };
        await mediator.ExecuteAsync(requestWithoutResponse, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(responseForRequestWithResponse);
            loggerMocker.VerifyInformationCall(times: Times.Exactly(2));
        });
    }
}
