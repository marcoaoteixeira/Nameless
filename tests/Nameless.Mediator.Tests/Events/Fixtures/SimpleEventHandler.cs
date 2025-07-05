using Microsoft.Extensions.Logging;

namespace Nameless.Mediator.Events.Fixtures;

public class SimpleEventHandler : IEventHandler<SimpleEvent> {
    private readonly ILogger _logger;

    public SimpleEventHandler(ILogger logger) {
        _logger = logger;
    }

    public Task HandleAsync(SimpleEvent evt, CancellationToken cancellationToken) {
        _logger.LogDebug("{EventHandler}: {Message}", GetType().Name, evt.Message);

        return Task.CompletedTask;
    }
}