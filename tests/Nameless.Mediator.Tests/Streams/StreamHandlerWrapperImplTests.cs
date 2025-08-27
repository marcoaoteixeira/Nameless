using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Mediator.Streams.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Streams;
public class StreamHandlerWrapperImplTests {
    [Fact]
    public async Task WhenHandlingStream_ThenExecuteStreamHandler() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();
        var stream = new SimpleStream { Messages = [nameof(StreamHandlerWrapperImplTests)] };
        var handler = new SimpleStreamHandler(loggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([])
                                                         .WithGetService<IStreamHandler<SimpleStream, string>>(handler)
                                                         .Build();
        var sut = new StreamHandlerWrapperImpl<SimpleStream, string>();

        // act
        var enumerable = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);
        var enumerator = enumerable.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        await enumerator.MoveNextAsync();

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains(enumerator.Current), Times.Once());
    }

    [Fact]
    public async Task WhenHandlingStream_WithMultipleResults_ThenExecuteStreamHandler() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();
        var stream = new SimpleStream {
            Messages = [
            nameof(StreamHandlerWrapperImplTests),
            nameof(StreamHandlerWrapperImplTests),
            nameof(StreamHandlerWrapperImplTests),
            nameof(StreamHandlerWrapperImplTests),
            nameof(StreamHandlerWrapperImplTests)
        ]
        };
        var handler = new SimpleStreamHandler(loggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([])
                                                         .WithGetService<IStreamHandler<SimpleStream, string>>(handler)
                                                         .Build();
        var sut = new StreamHandlerWrapperImpl<SimpleStream, string>();

        // act
        var enumerable = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);

        var resultList = new List<string>();
        await foreach (var item in enumerable) {
            resultList.Add(item);
        }

        // assert
        Assert.Multiple(() => {
            Assert.Equal(5, resultList.Count);
            loggerMocker.VerifyDebugCall(message => message.Contains(resultList[0]), Times.AtLeastOnce());
        });
    }

    [Fact]
    public async Task WhenHandlingStream_WithObjectStream_ThenExecuteStreamHandler() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();
        object stream = new SimpleStream { Messages = [nameof(StreamHandlerWrapperImplTests)] };
        var handler = new SimpleStreamHandler(loggerMocker.Build());

        var serviceProvider = new ServiceProviderMocker().WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([])
                                                         .WithGetService<IStreamHandler<SimpleStream, string>>(handler)
                                                         .Build();
        var sut = new StreamHandlerWrapperImpl<SimpleStream, string>();

        // act
        var enumerable = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);
        var enumerator = enumerable.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        await enumerator.MoveNextAsync();

        // assert
        loggerMocker.VerifyDebugCall(message => message.Contains((string)enumerator.Current), Times.Once());
    }

    [Fact]
    public async Task WhenHandlingStream_WhenThereAreStreamPipelineBehavior_ThenExecutePipelineBeforeStreamHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(logMessages.Add, LogLevel.Debug);
        var logger = loggerMocker.Build();

        var stream = new SimpleStream { Messages = [nameof(StreamHandlerWrapperImplTests)] };
        var streamHandler = new SimpleStreamHandler(logger);
        var streamPipelineBehavior = new SimpleStreamPipelineBehavior(logger);

        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<SimpleStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([streamPipelineBehavior])
                                                         .Build();

        var sut = new StreamHandlerWrapperImpl<SimpleStream, string>();

        // act
        var enumerable = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);
        var enumerator = enumerable.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        await enumerator.MoveNextAsync();

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(stream.Messages[0]), Times.Exactly(2));
            Assert.Contains(nameof(SimpleStreamPipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(SimpleStreamHandler), logMessages[1]);
        });
    }

    [Fact]
    public async Task WhenHandlingStream_WhenThereAreStreamPipelineBehavior_ThenExecutePipelineInOrderBeforeStreamHandler() {
        // arrange
        var logMessages = new List<string>();
        var loggerMocker = new LoggerMocker<object>().WithLogCallback(logMessages.Add, LogLevel.Debug);
        var logger = loggerMocker.Build();

        var stream = new SimpleStream { Messages = [nameof(StreamHandlerWrapperImplTests)] };
        var streamHandler = new SimpleStreamHandler(logger);

        IStreamPipelineBehavior<SimpleStream, string>[] streamPipelineBehaviors = [
            new SimpleStreamPipelineBehavior(logger),
            new YetAnotherSimpleStreamPipelineBehavior(logger)
        ];

        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<SimpleStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>(streamPipelineBehaviors)
                                                         .Build();

        var sut = new StreamHandlerWrapperImpl<SimpleStream, string>();

        // act
        var enumerable = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);
        var enumerator = enumerable.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        await enumerator.MoveNextAsync();

        // assert
        Assert.Multiple(() => {
            loggerMocker.VerifyDebugCall(message => message.Contains(stream.Messages[0]), Times.Exactly(3));
            Assert.Contains(nameof(SimpleStreamPipelineBehavior), logMessages[0]);
            Assert.Contains(nameof(YetAnotherSimpleStreamPipelineBehavior), logMessages[1]);
            Assert.Contains(nameof(SimpleStreamHandler), logMessages[2]);
        });
    }
}
