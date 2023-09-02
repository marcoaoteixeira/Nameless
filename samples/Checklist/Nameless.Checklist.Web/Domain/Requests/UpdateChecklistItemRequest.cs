using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests {
    public sealed class UpdateChecklistItemRequest : IRequest<ChecklistItemDto?> {
        #region Public Properties

        public Guid Id { get; init; }
        public string Description { get; init; } = null!;
        public DateTime? CheckedAt { get; init; }

        #endregion
    }
}
