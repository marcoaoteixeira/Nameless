using FluentValidation;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo;

public class CreateToDoInputValidator : AbstractValidator<CreateToDoInput> {
    public CreateToDoInputValidator() {
        RuleFor(input => input.Summary)
            .NotEmpty()
            .WithMessage("Summary cannot be empty.");

        RuleFor(input => input.DueDate)
            .NotNull()
            .WithMessage("DueDate cannot be empty.");
    }
}