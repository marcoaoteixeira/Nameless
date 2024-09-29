using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed record UpdateChecklistItemRequest : IRequest<ChecklistItemDto?> {
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime? CheckedAt { get; init; }
}