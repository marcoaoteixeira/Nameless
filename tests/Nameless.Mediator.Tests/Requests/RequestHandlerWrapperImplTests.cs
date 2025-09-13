using Nameless.Mediator.Fixtures;
using Nameless.Mediator.Requests.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.DependencyInjection;

namespace Nameless.Mediator.Requests;

[UnitTest]
public class RequestHandlerWrapperImplTests {
    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlers_ThenExecuteRequestHandler() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var requestHandler = new MessageRequestHandler(printServiceMock.Build());
        var request = new MessageRequest(nameof(RequestHandlerWrapperImplTests));
        var serviceProvider = new ServiceProviderMocker()
                              .WithGetService<IRequestHandler<MessageRequest, MessageResponse>>(requestHandler)
                              .WithGetService<IEnumerable<IRequestPipelineBehavior<MessageRequest, MessageResponse>>>(
                                  [])
                              .Build();

        var sut = new RequestHandlerWrapperImpl<MessageRequest, MessageResponse>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        printServiceMock.VerifyPrintCall(message => message.Contains(request.Message));
    }

    [Fact]
    public async Task
        WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_WhenThereAreRequestPipelineBehavior_ThenExecutePipelineBeforeRequestHandler() {
        // arrange
        var callbackResult = new List<string>();
        var printServiceMock = new PrintServiceMocker().WithPrintCallback(callbackResult.Add);
        var printService = printServiceMock.Build();
        var requestHandler = new MessageRequestHandler(printService);
        var request = new MessageRequest(nameof(RequestHandlerWrapperImplTests));
        var requestPipelineBehavior = new MessageRequestPipelineBehavior(printService);

        IRequestPipelineBehavior<MessageRequest, MessageResponse>[] pipelineBehaviors = [
            requestPipelineBehavior
        ];

        var serviceProvider = new ServiceProviderMocker()
                              .WithGetService<IRequestHandler<MessageRequest, MessageResponse>>(requestHandler)
                              .WithGetService<IEnumerable<IRequestPipelineBehavior<MessageRequest, MessageResponse>>>(
                                  pipelineBehaviors)
                              .Build();

        var sut = new RequestHandlerWrapperImpl<MessageRequest, MessageResponse>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.Contains(nameof(MessageRequestPipelineBehavior), callbackResult[index: 0]);
            Assert.Contains(nameof(MessageRequestHandler), callbackResult[index: 1]);
        });
    }
}