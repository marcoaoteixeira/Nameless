using System.Text.Json.Nodes;

namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Transforms an older schema version of an event's payload into the
///     next schema version, allowing the event store to keep persisted
///     events untouched while their CLR shape evolves.
/// </summary>
public interface IEventUpcaster {
    /// <summary>
    ///     Gets the stable name of the event type this upcaster applies
    ///     to, matching <see cref="EventTypeAttribute.Name"/>.
    /// </summary>
    string EventType { get; }

    /// <summary>
    ///     Gets the schema version this upcaster transforms from. The
    ///     result is at <c>FromVersion + 1</c>.
    /// </summary>
    int FromVersion { get; }

    /// <summary>
    ///     Transforms <paramref name="payload"/> from <see cref="FromVersion"/>
    ///     to the next schema version.
    /// </summary>
    /// <param name="payload">
    ///     The payload at <see cref="FromVersion"/>.
    /// </param>
    /// <returns>
    ///     The payload transformed to <c>FromVersion + 1</c>.
    /// </returns>
    JsonNode Upcast(JsonNode payload);
}