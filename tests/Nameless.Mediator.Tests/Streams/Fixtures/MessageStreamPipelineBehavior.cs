using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Streams.Fixtures;

public class MessageStreamPipelineBehavior : IStreamPipelineBehavior<MessageStream, string> {
    private readonly IPrintService _printService;

    public MessageStreamPipelineBehavior(IPrintService printService) {
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