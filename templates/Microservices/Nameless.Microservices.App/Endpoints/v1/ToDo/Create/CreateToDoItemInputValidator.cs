using FluentValidation;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed class CreateToDoItemInputValidator : AbstractValidator<CreateToDoItemInput> {
    public CreateToDoItemInputValidator() {
        RuleFor(item => item.Summary).NotEmpty();
        RuleFor(item => item.DueDate).GreaterThan(new DateTime(1900, 1, 1));
    }
}
