using Nameless.Mediator.Requests.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Requests;
public class RequestHandlerInvokerTests {
    [Fact]
    public async Task WhenExecutingRequest_ThenRelatedRequestHandlerShouldBeExecuted() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();

        var request = new SimpleRequest { Message = nameof(RequestHandlerInvokerTests) };
        var requestHandler = new SimpleRequestHandler(loggerMocker.Build());
        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequest>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequest, Nothing>>>([])
                                                         .Build();

        var sut = new RequestHandlerInvoker(serviceProvider);

        // act
        await sut.ExecuteAsync(request, CancellationToken.None);

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains(request.Message));
    }

    [Fact]
    public async Task WhenExecutingRequestWithResponse_ThenRelatedRequestHandlerShouldBeExecuted() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();

        var request = new SimpleRequestResponse { Message = nameof(RequestHandlerInvokerTests) };
        var requestHandler = new SimpleRequestResponseHandler(loggerMocker.Build());
        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequestResponse, string>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequestResponse, string>>>([])
                                                         .Build();

        var sut = new RequestHandlerInvoker(serviceProvider);

        // act
        var actual = await sut.ExecuteAsync(request, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(request.Message, actual);
            loggerMocker.VerifyDebugCall(message => message.Contains(request.Message));
        });
    }
}
