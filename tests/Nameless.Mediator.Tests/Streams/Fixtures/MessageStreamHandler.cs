using System.Runtime.CompilerServices;
using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Streams.Fixtures;

public class MessageStreamHandler : IStreamHandler<MessageStream, string> {
    private readonly IPrintService _printService;

    public MessageStreamHandler(IPrintService printService) {
        _printService = printService;
    }

    public async IAsyncEnumerable<string> HandleAsync(MessageStream request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        foreach (var message in request.Messages) {
            _printService.Print($"StreamHandler: {GetType().Name} | Request: {request.GetType().Name} | Message: {message}");

            yield return message;
        }

        await Task.Yield();
    }
}