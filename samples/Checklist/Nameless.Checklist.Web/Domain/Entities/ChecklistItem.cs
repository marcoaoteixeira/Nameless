namespace Nameless.Checklist.Web.Domain.Entities;

public sealed record ChecklistItem {
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime? CheckedAt { get; init; }

    public ChecklistItem() {
        Id = Guid.NewGuid();
    }

    public ChecklistItem(Guid id) {
        Id = id;
    }
}