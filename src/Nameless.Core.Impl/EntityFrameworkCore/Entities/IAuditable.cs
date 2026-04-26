namespace Nameless.EntityFrameworkCore.Entities;

public interface IAuditable {
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? ModificationDate { get; set; }
}