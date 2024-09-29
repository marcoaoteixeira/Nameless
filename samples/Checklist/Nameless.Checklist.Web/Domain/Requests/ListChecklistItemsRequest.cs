using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed record ListChecklistItemsRequest : IRequest<ChecklistItemDto[]> {
    public string? DescriptionLike { get; init; }
    public DateTime? CheckedBefore { get; init; }
}