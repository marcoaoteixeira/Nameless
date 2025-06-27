using Nameless.Mediator.Streams.Fixtures;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Streams;

public class StreamHandlerInvokerTests {
    [Fact]
    public async Task WhenCreatingStream_WhenThereAreStreamHandlers_ThenExecuteHandler() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();

        var request = new SimpleStream {
            Messages = [nameof(StreamHandlerInvokerTests)]
        };
        var streamHandler = new SimpleStreamHandler(loggerMocker.Build());
        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<SimpleStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([])
                                                         .Build();
        var sut = new StreamHandlerInvoker(serviceProvider);

        // act
        var actual = sut.CreateAsync(request, CancellationToken.None);
        var enumerator = actual.GetAsyncEnumerator(TestContext.Current.CancellationToken);
        var canMoveNext = await enumerator.MoveNextAsync();

        // assert
        Assert.True(canMoveNext);

        if (enumerator is IAsyncDisposable disposable) {
            await disposable.DisposeAsync();
        }
    }

    [Fact]
    public async Task WhenCreatingStream_WhenThereArePipelineBehaviors_WhenThereAreStreamHandlers_ThenExecutePipelineBeforeHandler() {
        // arrange
        var loggerMocker = new LoggerMocker<object>();

        var request = new SimpleStream {
            Messages = [nameof(StreamHandlerInvokerTests)]
        };
        var streamHandler = new SimpleStreamHandler(loggerMocker.Build());
        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<SimpleStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<SimpleStream, string>>>([])
                                                         .Build();
        var sut = new StreamHandlerInvoker(serviceProvider);

        // act
        var actual = sut.CreateAsync(request, CancellationToken.None);
        var enumerator = actual.GetAsyncEnumerator(TestContext.Current.CancellationToken);
        var canMoveNext = await enumerator.MoveNextAsync();

        // assert
        Assert.True(canMoveNext);

        if (enumerator is IAsyncDisposable disposable) {
            await disposable.DisposeAsync();
        }
    }
}
