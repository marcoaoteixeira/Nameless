using FluentValidation;

namespace Nameless.Validation.FluentValidation.Fixtures;

public class AnimalValidator : AbstractValidator<Animal> {
    public AnimalValidator() {
        RuleFor(item => item.Name)
           .NotEmpty();
    }
}