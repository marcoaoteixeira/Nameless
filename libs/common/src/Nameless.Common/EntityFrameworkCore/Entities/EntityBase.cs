namespace Nameless.EntityFrameworkCore.Entities;

/// <summary>
///     Base class for entities with UUID identifier.
/// </summary>
public abstract class EntityBase : IEntity<Guid> {
    /// <inheritdoc />
    public Guid ID { get; set; }

    /// <inheritdoc />
    public EntityState EntityState { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? CreationDate { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? ModificationDate { get; set; }
}