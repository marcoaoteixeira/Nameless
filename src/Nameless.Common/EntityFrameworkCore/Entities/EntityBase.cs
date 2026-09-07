namespace Nameless.EntityFrameworkCore.Entities;

/// <summary>
///     Base class for auditable entities with a strongly-typed identifier.
/// </summary>
/// <typeparam name="TID">
///     The type of the entity identifier; must be a value type that implements
///     <see cref="IEquatable{T}"/>.
/// </typeparam>
public abstract class EntityBase<TID> : IAuditable
    where TID : struct, IEquatable<TID> {
    /// <summary>
    ///     Gets or sets the unique identifier of the entity.
    /// </summary>
    public TID ID { get; set; }

    /// <summary>
    ///     Gets or sets the entity state.
    /// </summary>
    public EntityState EntityState { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? CreationDate { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? ModificationDate { get; set; }
}