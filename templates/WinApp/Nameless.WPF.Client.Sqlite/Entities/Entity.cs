namespace Nameless.WPF.Client.Sqlite.Entities;

/// <summary>
///     Represents an entity.
/// </summary>
public abstract class Entity {
    /// <summary>
    ///     Gets or sets the ID.
    /// </summary>
    public virtual Guid ID { get; set; }

    /// <summary>
    ///     Gets or sets the creation time.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the last modified time.
    /// </summary>
    public virtual DateTimeOffset? ModifiedAt { get; set; }
}