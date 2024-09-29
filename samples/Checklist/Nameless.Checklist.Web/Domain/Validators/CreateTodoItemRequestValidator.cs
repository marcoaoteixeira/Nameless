using FluentValidation;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Validators;

public sealed class CreateTodoItemRequestValidator : AbstractValidator<CreateChecklistItemRequest> {
    public CreateTodoItemRequestValidator() {
        RuleFor(request => request.Description).NotEmpty();
    }
}