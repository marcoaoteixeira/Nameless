namespace Nameless.Checklist.Web.Domain.Dtos;

public sealed record ChecklistItemDto {
    #region Public Properties

    public Guid Id { get; init; }
    public string Description { get; init; } = null!;
    public DateTime? CheckedAt { get; init; }
    public bool Checked => CheckedAt.GetValueOrDefault() != DateTime.MinValue;

    #endregion
}