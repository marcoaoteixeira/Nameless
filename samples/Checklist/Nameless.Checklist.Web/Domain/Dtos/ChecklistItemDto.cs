namespace Nameless.Checklist.Web.Domain.Dtos;

public sealed record ChecklistItemDto {
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime? CheckedAt { get; init; }
    public bool Checked => CheckedAt.GetValueOrDefault() != DateTime.MinValue;
}