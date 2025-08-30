using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Requests.Fixtures;

public class YetAnotherMessageRequestPipelineBehavior : IRequestPipelineBehavior<MessageRequest, MessageResponse> {
    private readonly IPrintService _printService;

    public YetAnotherMessageRequestPipelineBehavior(IPrintService printService) {
        _printService = printService;
    }

    public Task<MessageResponse> HandleAsync(MessageRequest request, RequestHandlerDelegate<MessageResponse> next, CancellationToken cancellationToken) {
        _printService.Print($"RequestPipelineBehavior: {GetType().Name} | Request: {request.GetType().Name} | Message: {request.Message}");

        return next(cancellationToken);
    }
}