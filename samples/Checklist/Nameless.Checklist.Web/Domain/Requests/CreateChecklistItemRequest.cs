using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;

namespace Nameless.Checklist.Web.Domain.Requests;

public sealed class CreateChecklistItemRequest : IRequest<ChecklistItemDto> {
    #region Public Properties

    public string Description { get; set; } = null!;

    #endregion
}