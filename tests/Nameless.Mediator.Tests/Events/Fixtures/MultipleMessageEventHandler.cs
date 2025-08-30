using Nameless.Mediator.Fixtures;

namespace Nameless.Mediator.Events.Fixtures;

public class MultipleMessageEventHandler : IEventHandler<MessageOneEvent>, IEventHandler<MessageTwoEvent> {
    private readonly IPrintService _printService;

    public MultipleMessageEventHandler(IPrintService printService) {
        _printService = printService;
    }

    public Task HandleAsync(MessageOneEvent evt, CancellationToken cancellationToken) {
        _printService.Print($"EventHandler: {GetType().Name} | Event: {evt.GetType().Name}");

        return Task.CompletedTask;
    }

    public Task HandleAsync(MessageTwoEvent evt, CancellationToken cancellationToken) {
        _printService.Print($"EventHandler: {GetType().Name} | Event: {evt.GetType().Name}");

        return Task.CompletedTask;
    }
}