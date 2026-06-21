using FluentValidation;
using Nameless.Microservices.Common.Chores.Inputs;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.Create;

public class CreateChoreInputValidator : AbstractValidator<CreateChoreInput> {
    public CreateChoreInputValidator() {
        RuleFor(input => input.Title).NotEmpty();
        RuleFor(input => input.Description).NotEmpty();
    }
}
