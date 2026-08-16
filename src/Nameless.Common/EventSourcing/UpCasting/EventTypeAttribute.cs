namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Marks a domain event class with the stable name and schema
///     version it is persisted under, decoupling the event's storage
///     identity from its CLR type name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EventTypeAttribute : Attribute {
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventTypeAttribute"/>
    ///     class.
    /// </summary>
    /// <param name="name">
    ///     The stable name identifying the event's type in the event
    ///     store.
    /// </param>
    /// <param name="version">
    ///     The current schema version of the event's payload.
    /// </param>
    public EventTypeAttribute(string name, int version) {
        Name = Throws.When.NullOrWhiteSpace(name);
        Version = Throws.When.LowerThan(version, compare: 1);
    }

    /// <summary>
    ///     Gets the stable name identifying the event's type in the
    ///     event store.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the current schema version of the event's payload.
    /// </summary>
    public int Version { get; }
}