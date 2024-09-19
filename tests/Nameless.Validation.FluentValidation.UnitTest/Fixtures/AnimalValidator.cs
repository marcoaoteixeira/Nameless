using FluentValidation;
using Nameless.Validation.FluentValidation.Fixtures;

namespace Nameless.Validation.FluentValidation.UnitTest.Fixtures;

public class AnimalValidator : AbstractValidator<Animal> {
    public AnimalValidator() {
        RuleFor(item => item.Name)
            .NotEmpty();
    }
}