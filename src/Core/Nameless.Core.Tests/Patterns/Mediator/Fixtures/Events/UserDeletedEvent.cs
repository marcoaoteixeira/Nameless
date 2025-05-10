using Nameless.Patterns.Mediator.Events;

namespace Nameless.Patterns.Mediator.Fixtures.Events;

public record UserDeletedEvent : IEvent {
    public User User { get; init; }
}
