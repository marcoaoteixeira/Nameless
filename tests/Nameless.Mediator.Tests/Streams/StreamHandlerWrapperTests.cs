using Nameless.Mediator.Fixtures;
using Nameless.Mediator.Streams.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mediator.Streams;

[UnitTest]
public class StreamHandlerWrapperTests {
    private readonly ITestOutputHelper _output;

    public StreamHandlerWrapperTests(ITestOutputHelper output) {
        _output = output;
    }

    [Fact]
    public async Task WhenCreatingStream_WhenThereAreStreamHandlers_ThenExecuteCorrectStreamHandler() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var streamHandler = new MessageStreamHandler(printServiceMock.Build());
        string[] messages = [
            "Message A",
            "Message B",
            "Message C",
        ];
        var stream = new MessageStream(Messages: messages);
        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<MessageStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<MessageStream, string>>>([])
                                                         .Build();

        var sut = new StreamHandlerWrapperImpl<MessageStream, string>();

        // act
        var actual = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);

        await foreach (var item in actual) {
            _output.WriteLine(item);
        }

        // assert
        printServiceMock.VerifyPrintCall(times: 3);
    }

    [Fact]
    public async Task WhenCreatingStream_WhenThereAreStreamHandlers_WhenThereIsOnePipeline_ThenExecutePipelineFirst_ThenExecuteStreamHandler() {
        // arrange
        var printServiceMock = new PrintServiceMocker();
        var printService = printServiceMock.Build();
        var streamHandler = new MessageStreamHandler(printService);
        string[] messages = [
            "Message A",
            "Message B",
            "Message C",
        ];
        var stream = new MessageStream(Messages: messages);
        var pipeline = new MessageStreamPipelineBehavior(printService);
        IStreamPipelineBehavior<MessageStream, string>[] pipelines = [
            pipeline
        ];
        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<MessageStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<MessageStream, string>>>(pipelines)
                                                         .Build();

        var sut = new StreamHandlerWrapperImpl<MessageStream, string>();

        // act
        var actual = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);

        await foreach (var item in actual) {
            _output.WriteLine(item);
        }

        // assert
        printServiceMock.VerifyPrintCall(times: 6);
        // Execute printService.Print
        //  3 times in pipeline
        //  3 times in stream handler
        //  Total of 6 times
    }

    [Fact]
    public async Task WhenCreatingStream_WhenThereAreStreamHandlers_WhenThereArePipelines_ThenExecutePipelinesInOrderFirst_ThenExecuteStreamHandler() {
        // arrange
        var printCallback = new List<string>();
        var printServiceMock = new PrintServiceMocker().WithPrintCallback(printCallback.Add);
        var printService = printServiceMock.Build();
        var streamHandler = new MessageStreamHandler(printService);
        string[] messages = [
            "Message A",
            "Message B",
            "Message C",
        ];
        var stream = new MessageStream(Messages: messages);
        var pipeline = new MessageStreamPipelineBehavior(printService);
        var yetAnotherPipeline = new YetAnotherMessageStreamPipelineBehavior(printService);
        IStreamPipelineBehavior<MessageStream, string>[] pipelines = [
            pipeline,
            yetAnotherPipeline
        ];
        var serviceProvider = new ServiceProviderMocker().WithGetService<IStreamHandler<MessageStream, string>>(streamHandler)
                                                         .WithGetService<IEnumerable<IStreamPipelineBehavior<MessageStream, string>>>(pipelines)
                                                         .Build();

        var sut = new StreamHandlerWrapperImpl<MessageStream, string>();

        // act
        var actual = sut.HandleAsync(stream, serviceProvider, CancellationToken.None);

        await foreach (var item in actual) {
            _output.WriteLine(item);
        }

        // assert
        Assert.Multiple(() => {
            Assert.Contains(nameof(MessageStreamPipelineBehavior), printCallback[0]);
            Assert.Contains(nameof(MessageStreamPipelineBehavior), printCallback[1]);
            Assert.Contains(nameof(MessageStreamPipelineBehavior), printCallback[2]);

            Assert.Contains(nameof(YetAnotherMessageStreamPipelineBehavior), printCallback[3]);
            Assert.Contains(nameof(YetAnotherMessageStreamPipelineBehavior), printCallback[4]);
            Assert.Contains(nameof(YetAnotherMessageStreamPipelineBehavior), printCallback[5]);

            Assert.Contains(nameof(MessageStreamHandler), printCallback[6]);
            Assert.Contains(nameof(MessageStreamHandler), printCallback[7]);
            Assert.Contains(nameof(MessageStreamHandler), printCallback[8]);

            printServiceMock.VerifyPrintCall(times: 9);
            // Execute printService.Print
            //  3 times in first pipeline
            //  3 times in second pipeline
            //  3 times in stream handler
            //  Total of 9 times
        });
    }
}
