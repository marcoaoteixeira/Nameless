using FluentValidation;
using Nameless.Web.Identity.Requests;

namespace Nameless.Web.Identity.Validators;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest> {
    public ConfirmEmailRequestValidator() {
        RuleFor(model => model.UserID)
            .NotEmpty()
            .WithMessage("Must provide user ID.");

        RuleFor(model => model.Code)
            .NotEmpty()
            .WithMessage("Must provide email confirmation code.");
    }
}