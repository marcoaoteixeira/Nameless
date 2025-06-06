namespace Nameless.Microservices.Domains.Common;
public abstract class AuditableEntity<TID> : Entity<TID>
    where TID : IEqualityComparer<TID> {
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    public virtual string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public virtual DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who last updated the entity.
    /// </summary>
    public virtual string? UpdatedBy { get; set; }
}
