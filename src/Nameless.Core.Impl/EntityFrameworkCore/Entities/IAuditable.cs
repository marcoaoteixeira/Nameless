namespace Nameless.EntityFrameworkCore.Entities;

/// <summary>
///     Marks an entity as auditable, providing timestamps for creation and
///     last modification, which are automatically filled by
///     <see cref="AuditableSaveChangesInterceptor"/>.
/// </summary>
public interface IAuditable {
    /// <summary>
    ///     Gets or sets the UTC date and time at which the entity was created.
    /// </summary>
    public DateTimeOffset? CreationDate { get; set; }

    /// <summary>
    ///     Gets or sets the UTC date and time at which the entity was last modified.
    /// </summary>
    public DateTimeOffset? ModificationDate { get; set; }
}