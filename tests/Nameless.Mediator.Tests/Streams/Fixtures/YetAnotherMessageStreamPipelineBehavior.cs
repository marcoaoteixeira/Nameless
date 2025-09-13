using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Streams.Fixtures;

public class YetAnotherMessageStreamPipelineBehavior : IStreamPipelineBehavior<MessageStream, string> {
    private readonly IPrintService _printService;

    public YetAnotherMessageStreamPipelineBehavior(IPrintService printService) {
        _printService = printService;
    }

    public IAsyncEnumerable<string> HandleAsync(MessageStream request, StreamHandlerDelegate<string> next,
        CancellationToken cancellationToken) {
        foreach (var message in request.Messages) {
            _printService.Print(
                $"StreamPipelineBehavior: {GetType().Name} | Request: {request.GetType().Name} | Message: {message}");
        }

        return next();
    }
}