using FluentValidation;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Validators;

public sealed class CreateTodoItemRequestValidator : AbstractValidator<CreateChecklistItemRequest> {
    #region Public Constructors

    public CreateTodoItemRequestValidator() {
        RuleFor(_ => _.Description)
            .NotEmpty();
    }

    #endregion
}