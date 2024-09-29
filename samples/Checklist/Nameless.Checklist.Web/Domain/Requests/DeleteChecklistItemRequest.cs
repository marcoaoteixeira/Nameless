using MediatR;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed record DeleteChecklistItemRequest : IRequest<bool> {
    public Guid Id { get; init; }
}