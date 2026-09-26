using Nameless.Mediator.Events;

namespace Nameless.EventSourcing;

/// <summary>
///     Provides a base class for event-sourced aggregates: state is
///     never persisted directly, it is derived by replaying the events
///     the aggregate has raised.
/// </summary>
/// <typeparam name="TID">
///     Type of the aggregate's identifier.
/// </typeparam>
public abstract class AggregateRoot<TID>
    where TID : notnull {
    private readonly List<IEvent> _uncommittedEvents = [];

    /// <summary>
    ///     Gets the aggregate's identifier.
    /// </summary>
    public TID ID { get; protected set; } = default!;

    /// <summary>
    ///     Gets the aggregate's current version, i.e. the number of
    ///     events that have been applied to it.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    ///     Gets a snapshot of the events raised since this aggregate was
    ///     loaded, not yet appended to the event store.
    /// </summary>
    /// <remarks>
    ///     Returns a copy: callers that hold on to a previous result
    ///     will not see it change once <see cref="ClearUncommittedEvents"/>
    ///     is called.
    /// </remarks>
    public IReadOnlyList<IEvent> UncommittedEvents => _uncommittedEvents.ToArray();

    /// <summary>
    ///     Rebuilds the aggregate's state by replaying its full event
    ///     history, without tracking any of them as uncommitted.
    /// </summary>
    /// <param name="history">
    ///     The events to replay, in the order they were originally
    ///     raised.
    /// </param>
    public void LoadFromHistory(IEnumerable<IEvent> history) {
        ArgumentNullException.ThrowIfNull(history);

        foreach (var @event in history) {
            When(@event);

            Version++;
        }
    }

    /// <summary>
    ///     Applies <paramref name="event"/> to the aggregate's state and
    ///     tracks it as an uncommitted event to be appended on the next
    ///     save.
    /// </summary>
    /// <param name="event">
    ///     The event being raised.
    /// </param>
    protected void RaiseEvent(IEvent @event) {
        Throws.When.Null(@event);

        When(@event);
        
        Version++;

        _uncommittedEvents.Add(@event);
    }

    /// <summary>
    ///     Marks all uncommitted events as committed, clearing them from
    ///     <see cref="UncommittedEvents"/>. Called once they have been
    ///     successfully appended to the event store.
    /// </summary>
    public void ClearUncommittedEvents() {
        _uncommittedEvents.Clear();
    }

    /// <summary>
    ///     Applies <paramref name="event"/> to the aggregate's in-memory
    ///     state. Implemented by concrete aggregates, typically as a
    ///     <see langword="switch"/> over the event's type.
    /// </summary>
    /// <param name="event">
    ///     The event to apply.
    /// </param>
    /// <remarks>
    ///     This method's shape is intentionally stable: a future source
    ///     generator will be able to generate it from <c>[Apply]</c>-annotated
    ///     methods without changing how <see cref="AggregateRoot{TID}"/>
    ///     is consumed.
    /// </remarks>
    protected abstract void When(IEvent @event);
}