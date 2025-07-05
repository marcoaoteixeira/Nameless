using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Mediator.Requests.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Requests;
public class RequestHandlerWrapperImplTests {
    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_ThenExecuteRequestHandler() {
        // arrange
        var requestHandlerLoggerMocker = new LoggerMocker<object>();

        var request = new SimpleRequest { Message = nameof(RequestHandlerWrapperImplTests) };
        var requestHandler = new SimpleRequestHandler(requestHandlerLoggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequest>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequest, Nothing>>>([])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequest>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        requestHandlerLoggerMocker.VerifyDebugCall(message => message.Contains(nameof(RequestHandlerWrapperImplTests)));
    }

    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_WhenThereAreRequestPipelineBehavior_ThenExecutePipelineBeforeRequestHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(LogLevel.Debug, logMessages.Add);
        var logger = loggerMocker.Build();

        var request = new SimpleRequest { Message = nameof(RequestHandlerWrapperImplTests) };
        var requestHandler = new SimpleRequestHandler(logger);

        var requestPipelineBehavior = new SimpleRequestPipelineBehavior(logger);

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequest>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequest, Nothing>>>([requestPipelineBehavior])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequest>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(request.Message), Times.Exactly(2));
            Assert.Contains(nameof(SimpleRequestPipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(SimpleRequestHandler), logMessages[1]);
        });
    }

    [Fact]
    public async Task WhenHandlingRequest_WhenThereAreRequestHandlersRegistered_WhenThereAreRequestPipelineBehavior_ThenExecutePipelineInOrderBeforeRequestHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(LogLevel.Debug, logMessages.Add);
        var logger = loggerMocker.Build();

        var request = new SimpleRequest { Message = nameof(RequestHandlerWrapperImplTests) };
        var requestHandler = new SimpleRequestHandler(logger);

        IRequestPipelineBehavior<SimpleRequest, Nothing>[] requestPipelineBehaviors = [
            new SimpleRequestPipelineBehavior(logger),
            new YetAnotherSimpleRequestPipelineBehavior(logger)
        ];

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequest>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequest, Nothing>>>(requestPipelineBehaviors)
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequest>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(request.Message), Times.Exactly(3));
            Assert.Contains(nameof(SimpleRequestPipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(YetAnotherSimpleRequestPipelineBehavior), logMessages[1]);
            Assert.Contains(nameof(SimpleRequestHandler), logMessages[2]);
        });
    }

    [Fact]
    public async Task WhenHandlingRequestWithNonGenericRequestObject_WhenThereAreRequestHandlersRegistered_ThenExecuteRequestHandler() {
        // arrange
        var requestHandlerLoggerMocker = new LoggerMocker<object>();

        object request = new SimpleRequest { Message = nameof(RequestHandlerWrapperImplTests) };
        var requestHandler = new SimpleRequestHandler(requestHandlerLoggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IRequestHandler<SimpleRequest>>(requestHandler)
                                                         .WithGetService<IEnumerable<IRequestPipelineBehavior<SimpleRequest, Nothing>>>([])
                                                         .Build();

        var sut = new RequestHandlerWrapperImpl<SimpleRequest>();

        // act
        _ = await sut.HandleAsync(request, serviceProvider, CancellationToken.None);

        // assert
        requestHandlerLoggerMocker.VerifyDebugCall(message => message.Contains(nameof(RequestHandlerWrapperImplTests)));
    }
}
