using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Nameless.WebApplication.Domain.v1.Users.Commands;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Validators {

    public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand> {

        #region Public Constructors

        public CreateUserCommandValidator(ApplicationDbContext dbContext) {
            Prevent.Null(dbContext, nameof(dbContext));

            RuleFor(_ => _.UserName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(256);

            RuleFor(_ => _.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256)
                .MustAsync(async (email, cancellationToken) => !await dbContext.Users.AnyAsync(_ => _.Email == email, cancellationToken))
                .WithMessage("User already exists with the same e-mail address.");

            RuleFor(_ => _.Password)
                .NotEmpty()
                .Matches(Constants.PASSWORD_REGEX_PATTERN);

            RuleFor(_ => _.ConfirmPassword)
                .Equal(input => input.Password);

            RuleFor(_ => _.PhoneNumber)
                .MaximumLength(64);
        }

        #endregion
    }
}
