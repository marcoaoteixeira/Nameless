using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests {
    public sealed class GetChecklistItemRequest : IRequest<ChecklistItemDto?> {
        #region Public Properties

        public Guid Id { get; init; }

        #endregion
    }
}
