using Nameless.Mediator.Events;

namespace Nameless.EventSourcing;

/// <summary>
///     Defines an append-only store for domain events.
/// </summary>
public interface IEventStore {
    /// <summary>
    ///     Appends <paramref name="events"/> to the stream identified by
    ///     <paramref name="streamID"/>.
    /// </summary>
    /// <param name="streamID">
    ///     The stream identifier, in the form
    ///     <c>{AggregateType}-{AggregateID}</c>.
    /// </param>
    /// <param name="aggregateType">
    ///     The discriminator of the aggregate type raising the events.
    /// </param>
    /// <param name="aggregateID">
    ///     The identifier of the aggregate instance raising the events.
    /// </param>
    /// <param name="tenantID">
    ///     The identifier of the tenant the events are scoped to, if any.
    /// </param>
    /// <param name="expectedVersion">
    ///     The version the caller expects the stream to currently be at,
    ///     used for optimistic concurrency. The first event appended
    ///     will be persisted at <c>expectedVersion + 1</c>.
    /// </param>
    /// <param name="events">
    ///     The events to append, in the order they were raised.
    /// </param>
    /// <param name="causedBy">
    ///     The identifier of the user or process that caused the events.
    /// </param>
    /// <param name="correlationID">
    ///     The identifier correlating the events with the request or
    ///     operation that produced them.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the append operation.
    /// </returns>
    /// <exception cref="ConcurrencyConflictException">
    ///     Thrown when the stream's current version does not match
    ///     <paramref name="expectedVersion"/>.
    /// </exception>
    Task AppendAsync(
        string streamID,
        string aggregateType,
        Guid aggregateID,
        Guid? tenantID,
        int expectedVersion,
        IReadOnlyList<IEvent> events,
        Guid causedBy,
        Guid correlationID,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Reads every event persisted for the stream identified by
    ///     <paramref name="streamID"/>, ordered by version.
    /// </summary>
    /// <param name="streamID">
    ///     The stream identifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     The stream's events, in version order. Empty when the stream
    ///     does not exist.
    /// </returns>
    Task<IReadOnlyList<EventEnvelope>> ReadStreamAsync(string streamID, CancellationToken cancellationToken);

    /// <summary>
    ///     Reads every event persisted across every stream, ordered by
    ///     <see cref="EventEnvelope.GlobalSequence"/>. Used to drive
    ///     projection rebuilds.
    /// </summary>
    /// <param name="fromGlobalSequence">
    ///     The exclusive lower bound of <see cref="EventEnvelope.GlobalSequence"/>
    ///     to start reading from.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     An asynchronous stream of every matching event, in order.
    /// </returns>
    IAsyncEnumerable<EventEnvelope> GetAsync(long fromGlobalSequence, CancellationToken cancellationToken);
}