namespace Nameless.EntityFrameworkCore.Entities;

public abstract class EntityBase<TID> : IAuditable
    where TID : struct, IEquatable<TID> {
    public TID ID { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? ModificationDate { get; set; }
}