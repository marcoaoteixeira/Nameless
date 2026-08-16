using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Converts between domain events and their persisted representation,
///     transparently applying <see cref="IEventUpcaster"/>s when reading
///     events stored under an older schema version.
/// </summary>
public interface IEventSerializer {
    /// <summary>
    ///     Serializes <paramref name="event"/> into its persisted
    ///     representation.
    /// </summary>
    /// <param name="event">
    ///     The event to serialize.
    /// </param>
    /// <returns>
    ///     The event's stable stored name, current schema version, and
    ///     JSON payload.
    /// </returns>
    EventMetadata Serialize(IEvent @event);

    /// <summary>
    ///     Deserializes an event's persisted representation back into its
    ///     current CLR type, upcasting <see cref="EventMetadata.Payload"/>
    ///     first if it was stored under an older
    ///     <see cref="EventMetadata.SchemaVersion"/>.
    /// </summary>
    /// <param name="metadata">
    ///     The event's metadata.
    /// </param>
    /// <returns>
    ///     The deserialized event.
    /// </returns>
    IEvent Deserialize(EventMetadata metadata);
}

/// <summary>
///     Represents the event metadata extract during serialization.
/// </summary>
/// <param name="EventType">
///     The event's stable stored name.
/// </param>
/// <param name="SchemaVersion">
///     The schema version <paramref name="Payload"/> was stored
///     under.
/// </param>
/// <param name="Payload">
///     The event's JSON payload.
/// </param>
public readonly record struct EventMetadata(string EventType, int SchemaVersion, string Payload);