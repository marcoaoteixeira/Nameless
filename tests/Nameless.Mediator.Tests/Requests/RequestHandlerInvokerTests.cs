using Nameless.Mediator.Fixtures;
using Nameless.Mediator.Requests.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.DependencyInjection;

namespace Nameless.Mediator.Requests;

[UnitTest]
public class RequestHandlerInvokerTests {
    [Fact]
    public async Task WhenExecutingRequest_ThenRequestHandlerAssociatedWithRequestShouldBeExecuted() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var requestHandler = new MessageRequestHandler(printServiceMock.Build());
        var request = new MessageRequest(nameof(RequestHandlerInvokerTests));
        var serviceProvider = new ServiceProviderMocker()
                              .WithGetService<IRequestHandler<MessageRequest, MessageResponse>>(requestHandler)
                              .WithGetService<IEnumerable<IRequestPipelineBehavior<MessageRequest, MessageResponse>>>(
                                  [])
                              .Build();

        var sut = new RequestHandlerInvoker(serviceProvider);

        // act
        await sut.ExecuteAsync(request, CancellationToken.None);

        // assert
        printServiceMock.VerifyPrintCall(message => message.Contains(request.Message));
    }
}