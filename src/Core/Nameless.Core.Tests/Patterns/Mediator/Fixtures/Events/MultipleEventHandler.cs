using Nameless.Patterns.Mediator.Events;

namespace Nameless.Patterns.Mediator.Fixtures.Events;

public class MultipleEventHandler : IEventHandler<UserCreatedEvent>,
                                    IEventHandler<UserDeletedEvent> {
    private readonly ObjectReceiver _objectReceiver;

    public MultipleEventHandler(ObjectReceiver objectReceiver) {
        _objectReceiver = objectReceiver;
    }

    public Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken) {
        _objectReceiver.Receive(evt.User);

        return Task.CompletedTask;
    }

    public Task HandleAsync(UserDeletedEvent evt, CancellationToken cancellationToken) {
        _objectReceiver.Receive(evt.User);

        return Task.CompletedTask;
    }
}
