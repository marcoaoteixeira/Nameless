using System.Collections.Concurrent;

namespace Nameless.CQRS.Events;

public interface IPublisher {
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent;
}