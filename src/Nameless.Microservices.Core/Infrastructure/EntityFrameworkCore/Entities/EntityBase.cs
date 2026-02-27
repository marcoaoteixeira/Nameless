namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore.Entities;

public abstract class EntityBase<TID>
    where TID : struct, IEquatable<TID> {
    public TID ID { get; set; }
}