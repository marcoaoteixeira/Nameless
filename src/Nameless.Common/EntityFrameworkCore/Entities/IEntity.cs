namespace Nameless.EntityFrameworkCore.Entities;

/// <summary>
///     Interface for EF Core entities.
/// </summary>
/// <typeparam name="TID">
///     The type of the entity identifier; must be a value type that implements
///     <see cref="IEquatable{T}"/>.
/// </typeparam>
public interface IEntity<TID> where TID : struct, IEquatable<TID> {
    /// <summary>
    ///     Gets or sets the unique identifier of the entity.
    /// </summary>
    TID ID { get; set; }

    /// <summary>
    ///     Gets or sets the entity state.
    /// </summary>
    EntityState EntityState { get; set; }

    /// <summary>
    ///     Gets or sets the creation date.
    /// </summary>
    DateTimeOffset? CreationDate { get; set; }

    /// <summary>
    ///     Gets or sets the modification date.
    /// </summary>
    DateTimeOffset? ModificationDate { get; set; }
}