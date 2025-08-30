using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Events.Fixtures;

public class MessageEventHandler : IEventHandler<MessageEvent> {
    private readonly IPrintService _printService;

    public MessageEventHandler(IPrintService printService) {
        _printService = printService;
    }

    public Task HandleAsync(MessageEvent evt, CancellationToken cancellationToken) {
        _printService.Print($"EventHandler: {GetType().Name} | Event: {evt.GetType().Name} | Message: {evt.Message}");

        return Task.CompletedTask;
    }
}