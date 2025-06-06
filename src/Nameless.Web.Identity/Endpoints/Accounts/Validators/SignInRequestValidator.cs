using FluentValidation;
using Nameless.Web.Identity.Endpoints.Accounts.Requests;

namespace Nameless.Web.Identity.Endpoints.Accounts.Validators;

public sealed class SignInRequestValidator : AbstractValidator<SignInRequest> {
    public SignInRequestValidator() {
        RuleFor(request => request.UserName).NotEmpty();
        RuleFor(request => request.Password).NotEmpty();
    }
}