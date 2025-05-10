using Nameless.Patterns.Mediator.Events;

namespace Nameless.Patterns.Mediator.Fixtures.Events;

public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent> {
    private readonly ObjectReceiver _objectReceiver;

    public UserCreatedEventHandler(ObjectReceiver objectReceiver) {
        _objectReceiver = objectReceiver;
    }

    public Task HandleAsync(UserCreatedEvent evt, CancellationToken cancellationToken) {
        _objectReceiver.Receive(evt.User);

        return Task.CompletedTask;
    }
}
