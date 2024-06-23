using FluentValidation;

namespace Nameless.Validation.FluentValidation.UnitTest.Fixtures {
    public class AnimalValidator : AbstractValidator<Animal> {
        public AnimalValidator() {
            RuleFor(item => item.Name)
                .NotEmpty();
        }
    }
}
