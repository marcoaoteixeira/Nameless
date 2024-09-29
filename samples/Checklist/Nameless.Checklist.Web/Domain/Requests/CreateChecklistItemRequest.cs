using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed record CreateChecklistItemRequest : IRequest<ChecklistItemDto> {
    public string Description { get; set; } = string.Empty;
}