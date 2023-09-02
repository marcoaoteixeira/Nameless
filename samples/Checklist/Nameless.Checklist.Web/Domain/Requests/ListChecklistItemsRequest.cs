using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests {
    public sealed class ListChecklistItemsRequest : IRequest<ChecklistItemDto[]> {
        #region Public Properties

        public string? DescriptionLike { get; init; }
        public DateTime? CheckedBefore { get; set; }

        #endregion
    }
}
