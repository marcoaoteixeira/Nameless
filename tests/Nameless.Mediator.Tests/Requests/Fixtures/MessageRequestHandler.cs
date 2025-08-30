using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Requests.Fixtures;

public class MessageRequestHandler : IRequestHandler<MessageRequest, MessageResponse> {
    private readonly IPrintService _printService;

    public MessageRequestHandler(IPrintService printService) {
        _printService = printService;
    }

    public Task<MessageResponse> HandleAsync(MessageRequest request, CancellationToken cancellationToken) {
        _printService.Print($"RequestHandler: {GetType().Name} | Request: {request.GetType().Name} | Message: {request.Message}");

        return Task.FromResult(new MessageResponse(request.Message));
    }
}