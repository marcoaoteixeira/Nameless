using FluentValidation;
using Nameless.Web.Identity.Api.Inputs;

namespace Nameless.Web.Identity.Api.Validators {
    public sealed class AuthenticateInputValidator : AbstractValidator<AuthenticateUserInput> {
        #region Public Constructors

        public AuthenticateInputValidator() {
            RuleFor(prop => prop.Username)
                .NotEmpty()
                .EmailAddress();

            RuleFor(prop => prop.Password)
                .NotEmpty();
        }

        #endregion
    }
}