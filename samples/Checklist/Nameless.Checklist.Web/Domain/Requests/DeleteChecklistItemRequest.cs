using MediatR;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed class DeleteChecklistItemRequest : IRequest<bool> {
    #region Public Properties

    public Guid Id { get; init; }

    #endregion
}