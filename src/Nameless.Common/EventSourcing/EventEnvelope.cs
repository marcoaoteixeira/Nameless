namespace Nameless.EventSourcing;

/// <summary>
///     Represents a single persisted record in an event stream, carrying
///     both the raw stored payload and the metadata needed for replay,
///     auditing, and optimistic concurrency.
/// </summary>
public sealed record EventEnvelope {
    /// <summary>
    ///     Gets the unique identifier of this event.
    /// </summary>
    public required Guid EventID { get; init; }

    /// <summary>
    ///     Gets the identifier of the stream this event belongs to, in the
    ///     form <c>{AggregateType}-{AggregateID}</c>.
    /// </summary>
    public required string StreamID { get; init; }

    /// <summary>
    ///     Gets the discriminator of the aggregate type that raised this
    ///     event.
    /// </summary>
    public required string AggregateType { get; init; }

    /// <summary>
    ///     Gets the identifier of the aggregate instance that raised this
    ///     event.
    /// </summary>
    public required Guid AggregateID { get; init; }

    /// <summary>
    ///     Gets the position of this event within its stream, starting
    ///     at 1.
    /// </summary>
    public required int Version { get; init; }

    /// <summary>
    ///     Gets the total order of this event across every stream,
    ///     used to drive projection rebuilds.
    /// </summary>
    public required long GlobalSequence { get; init; }

    /// <summary>
    ///     Gets the name identifying the event's type, used to resolve
    ///     the CLR type on deserialization.
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    ///     Gets the schema version of <see cref="Payload"/>, used to
    ///     determine which up-casters must run before deserialization.
    /// </summary>
    public required int EventSchemaVersion { get; init; }

    /// <summary>
    ///     Gets the raw JSON payload of the event, as originally stored.
    /// </summary>
    public required string Payload { get; init; }

    /// <summary>
    ///     Gets the identifier of the tenant this event is scoped to, if
    ///     any. A generic multi-tenancy partition column: consuming
    ///     applications decide what their top-level tenant is (a client,
    ///     a company, an organization, ...).
    /// </summary>
    public Guid? TenantID { get; init; }

    /// <summary>
    ///     Gets the identifier of the user or process that caused this
    ///     event.
    /// </summary>
    public required Guid CausedBy { get; init; }

    /// <summary>
    ///     Gets the identifier correlating this event with the request or
    ///     operation that produced it.
    /// </summary>
    public required Guid CorrelationID { get; init; }

    /// <summary>
    ///     Gets the point in time this event was appended to the store.
    /// </summary>
    public required DateTimeOffset OccurredAt { get; init; }
}