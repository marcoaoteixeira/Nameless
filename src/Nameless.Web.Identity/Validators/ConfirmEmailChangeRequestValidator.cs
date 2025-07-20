using FluentValidation;
using Nameless.Web.Identity.Requests;

namespace Nameless.Web.Identity.Validators;

public class ConfirmEmailChangeRequestValidator : AbstractValidator<ConfirmEmailChangeRequest> {
    public ConfirmEmailChangeRequestValidator() {
        RuleFor(model => model.UserID)
            .NotEmpty()
            .WithMessage("Must provide user ID.");

        RuleFor(model => model.Email)
            .EmailAddress()
            .WithMessage("Must provide email address.");

        RuleFor(model => model.Code)
            .NotEmpty()
            .WithMessage("Must provide email confirmation code.");
    }
}