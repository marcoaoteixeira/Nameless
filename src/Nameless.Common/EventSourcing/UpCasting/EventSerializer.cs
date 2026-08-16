using System.Text.Json;
using System.Text.Json.Nodes;
using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Default <see cref="IEventSerializer"/> implementation, backed by
///     <see cref="System.Text.Json"/> and a chain of
///     <see cref="IEventUpcaster"/>s resolved from <see cref="IEventTypeCatalog"/>.
/// </summary>
public sealed class EventSerializer : IEventSerializer {
    private readonly IEventTypeCatalog _typeCatalog;
    private readonly ILookup<(string EventType, int FromVersion), IEventUpcaster> _upCasters;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSerializer"/>
    ///     class.
    /// </summary>
    /// <param name="typeCatalog">
    ///     Resolves event types to their current CLR representation.
    /// </param>
    /// <param name="upCasters">
    ///     Every registered up-caster, applied in order until an event's
    ///     payload reaches its current schema version.
    /// </param>
    public EventSerializer(IEventTypeCatalog typeCatalog, IEnumerable<IEventUpcaster> upCasters) {
        _typeCatalog = Throws.When.Null(typeCatalog);
        _upCasters = Throws.When.Null(upCasters).ToLookup(upCaster => (upCaster.EventType, upCaster.FromVersion));
    }

    /// <inheritdoc/>
    public EventMetadata Serialize(IEvent @event) {
        Throws.When.Null(@event);

        var eventType = _typeCatalog.GetEventTypeName(@event.GetType());
        var schemaVersion = _typeCatalog.GetCurrentVersion(eventType);
        var payload = JsonSerializer.Serialize(@event, @event.GetType());

        return new EventMetadata(eventType, schemaVersion, payload);
    }

    /// <inheritdoc/>
    public IEvent Deserialize(EventMetadata metadata) {
        Throws.When.NullOrWhiteSpace(metadata.EventType);
        Throws.When.NullOrWhiteSpace(metadata.Payload);

        var currentVersion = _typeCatalog.GetCurrentVersion(metadata.EventType);
        var node = JsonNode.Parse(metadata.Payload) ??
                   throw new JsonException($"Event '{metadata.EventType}' payload is not valid JSON.");

        for (var version = metadata.SchemaVersion; version < currentVersion; version++) {
            var upCaster = _upCasters[(metadata.EventType, version)].FirstOrDefault() ??
                           throw new InvalidOperationException(
                               $"No up-caster registered to bring '{metadata.EventType}' from version '{version}' to '{version + 1}'."
                           );

            node = upCaster.Upcast(node);
        }

        var clrType = _typeCatalog.Resolve(metadata.EventType);

        return (IEvent)(node.Deserialize(clrType) ?? throw new JsonException($"Event '{metadata.EventType}' payload deserialized to null."));
    }
}