using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Mediator.Requests.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Requests;

public class RequestHandlerWrapperImplWithResponseTests {
    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_ThenExecuteRequestHandler() {
        // arrange
        var requestHandlerLoggerMocker = new LoggerMocker<object>();

        var request = new SimpleRequestResponse { Message = nameof(RequestHandlerWrapperImplWithResponseTests) };
        var requestHandler = new SimpleRequestResponseHandler(requestHandlerLoggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequestResponse, string>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequestResponse, string>>>([])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequestResponse, string>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        requestHandlerLoggerMocker.VerifyDebugCall(message => message.Contains(nameof(RequestHandlerWrapperImplWithResponseTests)));
    }

    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_WhenThereAreRequestPipelineBehavior_ThenExecutePipelineBeforeRequestHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(LogLevel.Debug, logMessages.Add);
        var logger = loggerMocker.Build();

        var request = new SimpleRequestResponse { Message = nameof(RequestHandlerWrapperImplWithResponseTests) };
        var requestHandler = new SimpleRequestResponseHandler(logger);

        var requestPipelineBehavior = new SimpleRequestResponsePipelineBehavior(logger);

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequestResponse, string>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequestResponse, string>>>([requestPipelineBehavior])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequestResponse, string>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(request.Message), Times.Exactly(2));
            Assert.Contains(nameof(SimpleRequestResponsePipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(SimpleRequestResponseHandler), logMessages[1]);
        });
    }

    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_WhenThereAreRequestPipelineBehavior_ThenExecutePipelineInOrderBeforeRequestHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(LogLevel.Debug, logMessages.Add);
        var logger = loggerMocker.Build();

        var request = new SimpleRequestResponse { Message = nameof(RequestHandlerWrapperImplWithResponseTests) };
        var requestHandler = new SimpleRequestResponseHandler(logger);

        IRequestPipelineBehavior<SimpleRequestResponse, string>[] requestPipelineBehaviors = [
            new SimpleRequestResponsePipelineBehavior(logger),
            new YetAnotherSimpleRequestResponsePipelineBehavior(logger)
        ];

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequestResponse, string>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequestResponse, string>>>(requestPipelineBehaviors)
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequestResponse, string>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(request.Message), Times.Exactly(3));
            Assert.Contains(nameof(SimpleRequestResponsePipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(YetAnotherSimpleRequestResponsePipelineBehavior), logMessages[1]);
            Assert.Contains(nameof(SimpleRequestResponseHandler), logMessages[2]);
        });
    }

    [Fact]
    public async Task WhenHandlingRequestWithNonGenericRequestObject_WhenThereAreRequestHandlersRegistered_ThenExecuteRequestHandler() {
        // arrange
        var requestHandlerLoggerMocker = new LoggerMocker<object>();

        object request = new SimpleRequestResponse { Message = nameof(RequestHandlerWrapperImplWithResponseTests) };
        var requestHandler = new SimpleRequestResponseHandler(requestHandlerLoggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequestResponse, string>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequestResponse, string>>>([])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequestResponse, string>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        requestHandlerLoggerMocker.VerifyDebugCall(message => message.Contains(nameof(RequestHandlerWrapperImplWithResponseTests)));
    }
}
