namespace Nameless.Checklist.Web.Domain.Entities;

public sealed record ChecklistItem {
    #region Public Properties

    public Guid Id { get; init; }
    public string Description { get; init; } = null!;
    public DateTime? CheckedAt { get; init; }

    #endregion

    #region Public Constructors

    public ChecklistItem() {
        Id = Guid.NewGuid();
    }

    public ChecklistItem(Guid id) {
        Id = id;
    }

    #endregion
}