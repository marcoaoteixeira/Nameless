using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed record GetChecklistItemRequest : IRequest<ChecklistItemDto?> {
    public Guid Id { get; init; }
}