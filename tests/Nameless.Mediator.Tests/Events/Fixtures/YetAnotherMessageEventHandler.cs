using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Events.Fixtures;

public class YetAnotherMessageEventHandler : IEventHandler<MessageEvent> {
    private readonly IPrintService _printService;

    public YetAnotherMessageEventHandler(IPrintService printService) {
        _printService = printService;
    }

    public Task HandleAsync(MessageEvent evt, CancellationToken cancellationToken) {
        _printService.Print($"EventHandler: {GetType().Name} | Event: {evt.GetType().Name}");

        return Task.CompletedTask;
    }
}