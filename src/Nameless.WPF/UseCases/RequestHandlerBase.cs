using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.UseCases;

public abstract class RequestHandlerBase<TSelf, TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TSelf : RequestHandlerBase<TSelf, TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    private readonly IMessenger _messenger;

    protected ILogger<TSelf> Logger { get; }

    protected RequestHandlerBase(IMessenger messenger, ILogger<TSelf> logger) {
        _messenger = messenger;

        Logger = logger;
    }

    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);

    protected Task NotifyInformationAsync(string content) {
        return NotifyAsync(content, MessageType.Information);
    }

    protected Task NotifySuccessAsync(string content) {
        return NotifyAsync(content, MessageType.Success);
    }

    protected Task NotifyWarningAsync(string content) {
        return NotifyAsync(content, MessageType.Warning);
    }

    protected Task NotifyFailureAsync(string content) {
        return NotifyAsync(content, MessageType.Failure);
    }

    private Task NotifyAsync(string content, MessageType type) {
        return _messenger.PublishAsync(new UseCaseMessage {
            Content = content,
            UseCase = GetType().Name,
            Type = type
        });
    }
}
