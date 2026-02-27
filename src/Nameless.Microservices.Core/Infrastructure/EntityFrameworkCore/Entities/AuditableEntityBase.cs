namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore.Entities;

public abstract class AuditableEntityBase<TID> : EntityBase<TID>, IAuditableEntity
    where TID : struct, IEquatable<TID> {
    public string? MostRecentAuditee { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? ModificationDate { get; set; }
}