using FluentValidation;
using Nameless.Sample.Chores.Inputs;

namespace Nameless.Sample.Chores.Validators;

public class PostChoreInputValidator : AbstractValidator<PostChoreInput> {
    public PostChoreInputValidator() {
        RuleFor(input => input.Description)
            .NotEmpty()
            .WithMessage("Description cannot be null, empty or white spaces.");

        RuleFor(input => input.Date)
            .NotEqual(default(DateOnly)).WithMessage("Date is required.")
            .GreaterThan(new DateOnly(2000, 1, 1)).WithMessage("Date cannot be lower than 2000-01-01")
            .LessThan(new DateOnly(2099, 12, 31)).WithMessage("Date cannot be greater than 2099-12-31");
    }
}
