namespace Nameless.Microservices.Domains.Common;

public abstract class Entity<TID>
    where TID : IEqualityComparer<TID> {

    private readonly List<EntityEvent> _events = [];

    /// <summary>
    /// Gets the list of domain events associated with the entity.
    /// </summary>
    public IEnumerable<EntityEvent> Events => _events.AsReadOnly();

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public virtual required TID ID { get; set; }

    /// <summary>
    /// Adds a domain event to the entity's event list.
    /// </summary>
    /// <param name="evt">The event.</param>
    public void AddEvent(EntityEvent evt) {
        _events.Add(evt);
    }

    /// <summary>
    /// Removes a domain event from the entity's event list.
    /// </summary>
    /// <param name="evt">The event.</param>
    public void RemoveEvent(EntityEvent evt) {
        _events.Remove(evt);
    }

    /// <summary>
    /// Removes all events from the entity's event list.
    /// </summary>
    public void ClearEvents() {
        _events.Clear();
    }
}
