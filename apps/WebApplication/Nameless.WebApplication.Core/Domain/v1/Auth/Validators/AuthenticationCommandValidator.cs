using FluentValidation;
using Nameless.WebApplication.Domain.v1.Auth.Commands;

namespace Nameless.WebApplication.Domain.v1.Auth.Validators {

    public sealed class AuthenticationCommandValidator : AbstractValidator<AuthenticationCommand> {

        #region Public Constructors

        public AuthenticationCommandValidator() {
            RuleFor(_ => _.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(_ => _.Password)
                .NotEmpty();
        }

        #endregion
    }
}