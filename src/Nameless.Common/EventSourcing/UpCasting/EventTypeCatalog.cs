using System.Reflection;

namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Default <see cref="IEventTypeCatalog"/> implementation, built once
///     from a fixed set of event types decorated with
///     <see cref="EventTypeAttribute"/>.
/// </summary>
public sealed class EventTypeCatalog : IEventTypeCatalog {
    private readonly Dictionary<string, (Type ClrType, int Version)> _byEventType;
    private readonly Dictionary<Type, string> _byClrType;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventTypeCatalog"/>
    ///     class from <paramref name="eventTypes"/>, keeping only those
    ///     decorated with <see cref="EventTypeAttribute"/>.
    /// </summary>
    /// <param name="eventTypes">
    ///     The candidate event types, typically discovered via
    ///     assembly scanning.
    /// </param>
    public EventTypeCatalog(IEnumerable<Type> eventTypes) {
        ArgumentNullException.ThrowIfNull(eventTypes);

        var entries = eventTypes
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<EventTypeAttribute>()))
            .Where(entry => entry.Attribute is not null)
            .ToList();

        _byEventType = entries.ToDictionary(
            keySelector: entry => entry.Attribute!.Name,
            elementSelector: entry => (entry.Type, entry.Attribute!.Version)
        );

        _byClrType = entries.ToDictionary(
            keySelector: entry => entry.Type,
            elementSelector: entry => entry.Attribute!.Name
        );
    }

    /// <inheritdoc/>
    public Type Resolve(string eventType) {
        return Find(eventType).ClrType;
    }

    /// <inheritdoc/>
    public int GetCurrentVersion(string eventType) {
        return Find(eventType).Version;
    }

    /// <inheritdoc/>
    public string GetEventTypeName(Type clrType) {
        Throws.When.Null(clrType);

        return _byClrType.TryGetValue(clrType, out var name)
            ? name
            : throw new KeyNotFoundException($"Type '{clrType}' is not decorated with '{nameof(EventTypeAttribute)}'.");
    }

    private (Type ClrType, int Version) Find(string eventType) {
        Throws.When.NullOrWhiteSpace(eventType);

        return _byEventType.TryGetValue(eventType, out var entry)
            ? entry
            : throw new KeyNotFoundException($"No event type registered for '{eventType}'.");
    }
}